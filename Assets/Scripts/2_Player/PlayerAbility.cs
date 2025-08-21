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
    [Header("Player ������")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("������ ��ų")]
    public Skill meleeSkill;
    public Skill rangedSkill;
    public Skill dash;
    public Skill skill1;
    public Skill skill2;

    [Header("�ĺ�")]
    public int playerNumber;

    // Re-peek ��
    private bool _hasLastActionType;
    private SkillType _lastActionType;

    // Health ���� (HP�� ������ ������)
    public PlayerHealth Health { get; private set; }

    // HP �̺�Ʈ �н�����(�ɼ������� UI�� ��)
    public event Action<int, int> OnHPChanged
    {
        add { if (Health != null) Health.OnHPChanged += value; }
        remove { if (Health != null) Health.OnHPChanged -= value; }
    }

    // ���Ժ� ��ٿ� ������ ���ǡ�
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

    private void Awake()
    {
        Health = GetComponent<PlayerHealth>();
    }

    // ======= ��ų ����/����/��ٿ� ============
    public void SetSkill(SkillType type, Skill skill)
    {
        if (skill == null) return;
        skill.SetNewBasicValue(this);
        skill.SetAssignedSlot(type);

        switch (type)
        {
            case SkillType.Melee: meleeSkill = skill; break;
            case SkillType.Ranged: rangedSkill = skill; break;
            case SkillType.Dash: dash = skill; break;
            case SkillType.Skill1: skill1 = skill; break;
            case SkillType.Skill2: skill2 = skill; break;
        }
    }

    public void TryExecuteSkill(SkillType type, Vector3 origin, Vector3 direction, bool force = false, float? overrideDuration = null)
    {
        var s = GetSkill(type);
        if (s == null) return;

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
        StartCooldown(type, duration);

        // �ٷ� ȿ�� ����
        s.Execute(origin, direction);

        if (!(s is SK_Repeek))
            RecordLastActionType(type);

        if (!force)
        {
            P2PMessageSender.SendMessage(
                SkillMessageBuilder.Build(origin, direction, type, playerNumber)
            );
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
}