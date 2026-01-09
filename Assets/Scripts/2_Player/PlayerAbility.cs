using System.Collections.Generic;
using UnityEngine;
using System;

public struct CooldownState
{
    public bool active;
    public float duration;
    public float endTime;

    public float Remaining => Mathf.Max(0f, endTime - Time.time);
    public float Normalized => duration > 0f ? Mathf.Clamp01(Remaining / duration) : 0f;
}

public class PlayerAbility : MonoBehaviour
{
    [Header("Player 움직임")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("장착된 스킬")]
    public Skill meleeSkill;
    public Skill rangedSkill;
    public Skill dash;
    public Skill skill1;
    public Skill skill2;

    [Header("패시브 스킬")]
    public AbilityEvents events;
    public List<Passive> passives = new();

    // 개발용
    [Header("Passives (Inspector Auto-Equip)")]
    public List<Passive> initialPassives = new List<Passive>();   // 인스펙터에 드래그

    [Header("식별")]
    public int playerNumber;

    [Header("스킬 애니메이션")]
    public SkillEffectAnimation effect;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;

    private PlayerMovement movement;

    [Header("AlrmArrow")]
    public GameObject obj_AlrmArrow;

    // Re-peek 용
    private bool _hasLastActionType;
    private SkillType _lastActionType;

    // 스킬 잠금용
    public bool SkillExecLocked { get; private set; }

    // Health 참조 (HP의 유일한 소유자)
    public PlayerHealth Health { get; private set; }


    // 대쉬 잔상 용
    public VFX_MeshTrail meshTrail;

    // HP 이벤트 패스스루(옵션이지만 UI에 편리)
    public event Action<int, int> OnHPChanged
    {
        add { if (Health != null) Health.OnHPChanged += value; }
        remove { if (Health != null) Health.OnHPChanged -= value; }
    }

    // 슬롯별 쿨다운 “단일 진실”
    private readonly Dictionary<SkillType, CooldownState> cooldowns =
        new Dictionary<SkillType, CooldownState>
        {
            { SkillType.Melee,  new CooldownState() },
            { SkillType.Ranged, new CooldownState() },
            { SkillType.Dash, new CooldownState() },
            { SkillType.Skill1, new CooldownState() },
            { SkillType.Skill2, new CooldownState() },
        };

    public event System.Action<SkillType> OnCooldownChanged;

    // 스킬 장착 알림 (UI 아이콘 주입용)
    public event System.Action<SkillType, Skill> OnSkillEquipped;
    public event System.Action<int, Passive> OnPassiveSlotChanged;

    private void Awake()
    {
        Health = GetComponent<PlayerHealth>();
        if (!events) events = GetComponent<AbilityEvents>();
        if (!meshTrail) meshTrail = GetComponent<VFX_MeshTrail>();
        movement = GetComponent<PlayerMovement>();

        ResolveSfxSource();
    }

    void Start()
    {
        AutoEquipInitialPassives(); // 개발용


        if (MatchResultStore.myPlayerNumber == playerNumber)
        {
            if (obj_AlrmArrow != null)
            {
                Vector3 spawnOffset = new Vector3(0.5f, 0f, 0f);

                // [수정] Quaternion.identity -> obj_AlrmArrow.transform.rotation
                GameObject arrowInstance = Instantiate(obj_AlrmArrow, transform.position + spawnOffset, obj_AlrmArrow.transform.rotation);

                AlrmArrow arrowScript = arrowInstance.GetComponent<AlrmArrow>();

                if (arrowScript != null)
                {
                    arrowScript.SetTarget(transform, spawnOffset);
                }
            }
        }

    }

    void AutoEquipInitialPassives()
    {
        foreach (var prefab in initialPassives)
        {
            if (!prefab) continue;

            var inst = (prefab.transform.root == transform.root)
                ? prefab
                : Instantiate(prefab, transform);
            if (!passives.Contains(inst))
            {
                inst.OnEquip(this);
                passives.Add(inst);
            }
        }
    }

    void Update()
    {
        events?.EmitTick(Time.deltaTime);

    }

    //private void FixedUpdate()
    //{
    //    Debug.Log(moveSpeed);
    //}

    // ======= 스킬 장착/실행/쿨다운 ============
    public void SetSkill(SkillType type, Skill skill)
    {
        if (skill == null) return;

        skill.transform.SetParent(this.transform);
        skill.transform.localPosition = Vector3.zero;

        skill.SetNewBasicValue(this);
        skill.Bind(this);
        skill.SetAssignedSlot(type);

        switch (type)
        {
            case SkillType.Melee: meleeSkill = skill; break;
            case SkillType.Ranged: rangedSkill = skill; break;
            case SkillType.Dash: dash = skill; break;
            case SkillType.Skill1: skill1 = skill; break;
            case SkillType.Skill2: skill2 = skill; break;
        }

        OnSkillEquipped?.Invoke(type, skill);
    }

    public void TryExecuteSkill(SkillType type, Vector3 origin, Vector3 direction, bool force = false, float? overrideDuration = null)
    {
        if (!force && SkillExecLocked) return;

        var s = GetSkill(type);
        if (s == null) return;

        if (type != SkillType.Dash)
        {
            SK_Ghostride ghost = null;

            if (meleeSkill is SK_Ghostride g1) ghost = g1;
            if (rangedSkill is SK_Ghostride g2) ghost = g2;
            if (skill1 is SK_Ghostride g4) ghost = g4;
            if (skill2 is SK_Ghostride g5) ghost = g5;

            if (ghost != null)
                ghost.NotifyAttack();
        }

        if (s is SK_Repeek && TryGetLastActionType(out var lastType))
        {
            var target = GetSkill(lastType);
            if (target != null)
                overrideDuration = target.coolTime;
        }

        var st = GetCooldown(type);
        bool onCooldown = st.active && st.Remaining > 0f;
        if (!force && onCooldown) return;

        float duration = Mathf.Max(0f, overrideDuration ?? s.coolTime);
        events?.EmitModifyCooldown(type, ref duration);
        events?.EmitCooldownFinalized(type, duration);
        StartCooldown(type, duration);

        Vector3 execDir = direction;

        // BullRush는 자동 조준 방향
        if (s is SK_BullRush bullRush)
        {
            execDir = bullRush.GetAutoDirection(direction);
        }

        // 바로 효과 실행
        s.Execute(origin, direction);
        if (effect != null) effect.PlayShakeAnimation(SlotToIndex(type));

        events?.EmitSkillExecuted(type);

        if (!(s is SK_Repeek))
            RecordLastActionType(type);

        if (movement != null)
        {
            if (s is SK_Dash dashSkill)
                movement.LockFacing(execDir, dashSkill.dashDuration);
            else if (s is SK_BullRush br)
                movement.LockFacing(execDir, br.dashDuration);
            else
                movement.LockFacing(execDir);
        }

        if (!force)
        {
            P2PMessageSender.SendMessage(
                SkillMessageBuilder.Build(origin, execDir, type, playerNumber)
            );
        }
    }

    private int SlotToIndex(SkillType s)
    {
        switch (s)
        {
            case SkillType.Melee: return 0;
            case SkillType.Ranged: return 1;
            case SkillType.Dash: return 2;
            case SkillType.Skill1: return 3;
            case SkillType.Skill2: return 4;
            default: return -1;
        }
    }

    public void StartCooldown(SkillType slot, float duration)
    {
        var st = cooldowns[slot];
        st.active = duration > 0f;
        st.duration = duration;
        st.endTime = Time.time + duration;
        cooldowns[slot] = st;
        OnCooldownChanged?.Invoke(slot);
    }

    public void CancelCooldown(SkillType slot)
    {
        var st = cooldowns[slot];
        st.active = false;
        st.duration = 0f;
        st.endTime = Time.time;
        cooldowns[slot] = st;
        OnCooldownChanged?.Invoke(slot);
    }
    public void ResetAllCooldowns()
    {
        var now = Time.time;
        var keys = new List<SkillType>(cooldowns.Keys);

        foreach (var slot in keys)
        {
            var st = cooldowns[slot];
            st.active = false;
            st.duration = 0f;
            st.endTime = now;
            cooldowns[slot] = st;

            OnCooldownChanged?.Invoke(slot);
        }
    }

    public CooldownState GetCooldown(SkillType slot) => cooldowns[slot];

    public Skill GetSkill(SkillType t) => t switch
    {
        SkillType.Melee => meleeSkill,
        SkillType.Ranged => rangedSkill,
        SkillType.Dash => dash,
        SkillType.Skill1 => skill1,
        SkillType.Skill2 => skill2,
        _ => null
    };

    public void RecordLastActionType(SkillType type)
    {
        _hasLastActionType = true;
        _lastActionType = type;
    }

    public bool TryGetLastActionType(out SkillType type)
    {
        type = _lastActionType;
        return _hasLastActionType;
    }

    public Passive EquipPassive(Passive prefab)
    {
        if (!prefab) return null;

        Passive inst = (prefab.transform.root == transform.root)
            ? prefab
            : Instantiate(prefab, transform);

        if (!passives.Contains(inst))
        {
            inst.OnEquip(this);
            passives.Add(inst);

            int slotIndex = Mathf.Clamp(passives.Count - 1, 0, 2);
            OnPassiveSlotChanged?.Invoke(slotIndex, inst);
        }

        return inst;
    }


    public void UnequipPassive(Passive p)
    {
        if (!p) return;
        int idx = passives.IndexOf(p);

        if (idx >= 0)
        {
            p.OnUnequip();
            passives.RemoveAt(idx);
            Destroy(p.gameObject);

            OnPassiveSlotChanged?.Invoke(idx, null);
        }
    }

    // PS_KickStart 용
    public void ReduceAllCooldowns(float seconds)
    {
        if (seconds <= 0f) return;

        var now = Time.time;
        var keys = new List<SkillType>(cooldowns.Keys);

        foreach (var slot in keys)
        {
            var st = cooldowns[slot];
            if (!st.active) continue;

            float remaining = Mathf.Max(0f, st.endTime - now);

            remaining = Mathf.Max(0f, remaining - seconds);

            if (remaining <= 0f)
            {
                st.active = false;
                st.endTime = now;
            }
            else
            {
                st.endTime = now + remaining;
            }

            cooldowns[slot] = st;
            OnCooldownChanged?.Invoke(slot); // UI 갱신 등
        }
    }

    // 원격 패시브용
    public Passive GetPassiveById(int passiveId)
    {
        if (passives == null) return null;

        for (int i = 0; i < passives.Count; i++)
        {
            var p = passives[i];
            if (!p) continue;
            if (p.PassiveId == passiveId) return p;
        }
        return null;
    }

    public void RemoteExecutePassive(PassiveProcMessage msg)
    {
        if (msg == null) return;

        var p = GetPassiveById(msg.passiveId);
        if (p == null) return;

        p.RemoteExecute(msg);
    }

    // ChronoChaos 용
    public void ApplyCooldownDelta(SkillType slot, float deltaSeconds)
    {
        if (Mathf.Abs(deltaSeconds) < 1e-6f) return;

        var st = cooldowns[slot];
        if (!st.active) return;

        float now = Time.time;
        float remaining = Mathf.Max(0f, st.endTime - now);

        float newRemaining = Mathf.Max(0f, remaining + deltaSeconds);

        float newDuration = Mathf.Max(0f, st.duration + deltaSeconds);

        st.duration = newDuration;
        st.endTime = now + newRemaining;
        st.active = newRemaining > 0f;

        cooldowns[slot] = st;
        OnCooldownChanged?.Invoke(slot);
    }

    private void ResolveSfxSource()
    {
        if (sfxSource) return;

        var go = GameObject.Find("Audio_SFX");
        if (go == null)
        {
            Debug.LogWarning("[PlayerAbility] Audio_SFX GameObject not found in scene");
            return;
        }

        sfxSource = go.GetComponent<AudioSource>();
        if (sfxSource == null)
        {
            Debug.LogWarning("[PlayerAbility] Audio_SFX has no AudioSource");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!clip || !sfxSource) return;
        sfxSource.PlayOneShot(clip);
    }

    public void NotifySkillUIChanged(SkillType slot)
    {
        var s = GetSkill(slot);
        if (s == null) return;
        OnSkillEquipped?.Invoke(slot, s);
    }

    public void SetSkillExecLocked(bool locked)
    {
        SkillExecLocked = locked;
    }
}