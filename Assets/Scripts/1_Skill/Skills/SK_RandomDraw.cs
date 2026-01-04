using System.Collections.Generic;
using UnityEngine;

public class SK_RandomDraw : Skill
{
    [Header("랜덤 후보 스킬 프리팹들")]
    [SerializeField] private List<Skill> candidateSkillPrefabs = new List<Skill>();

    [Header("랜덤 변경 주기 (초 단위)")]
    [SerializeField] private float changeInterval = 7f; 

    private readonly List<Skill> runtimeSkills = new List<Skill>();
    private Skill currentSkill;

    private bool skillsInitialized = false;
    private float nextChangeTime = 0f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    private void Update()
    {
        if (!playerAbility) return;

        if (playerAbility.playerNumber != MatchResultStore.myPlayerNumber)
        {
            if (!skillsInitialized)
            {
                InitRuntimeSkills();
            }
            return;
        }

        if (!skillsInitialized)
        {
            InitRuntimeSkills();
            RollInitialSkill();
            SendStateIfPossible();
        }

        // 7초마다 무조건 스킬 교체
        if (Time.time >= nextChangeTime)
        {
            RollNextSkill(resetCooldown: true);
            SendStateIfPossible();
        }
    }

    private void InitRuntimeSkills()
    {
        skillsInitialized = true;
        runtimeSkills.Clear();

        if (playerAbility == null)
        {
            Debug.LogWarning("[SK_RandomDraw] playerAbility가 없습니다.");
            return;
        }

        foreach (var prefab in candidateSkillPrefabs)
        {
            if (!prefab) continue;
            if (prefab == this)
            {
                Debug.LogWarning("[SK_RandomDraw] 자기 자신을 candidate에 넣으면 안 됩니다. 스킵합니다.");
                continue;
            }

            var inst = Instantiate(prefab, playerAbility.transform);

            inst.Bind(playerAbility);
            inst.SetNewBasicValue(playerAbility);
            inst.SetAssignedSlot(assignedSlot);

            inst.enabled = false;

            runtimeSkills.Add(inst);
        }
    }

    private void RollInitialSkill()
    {
        RollNextSkill(resetCooldown: false);

      
    }

    private void RollNextSkill(bool resetCooldown)
    {
        if (runtimeSkills.Count == 0) return;

        currentSkill = runtimeSkills[Random.Range(0, runtimeSkills.Count)];

        if (currentSkill != null)
        {
            coolTime = Mathf.Max(0f, currentSkill.coolTime);
            skillIcon = currentSkill.skillIcon;
        }

        // 7초마다 쿨타임 초기화
        if (resetCooldown && playerAbility != null)
        {
            playerAbility.CancelCooldown(assignedSlot);
        }

        if (playerAbility != null)
        {
            playerAbility.NotifySkillUIChanged(assignedSlot);
        }

        playerAbility.PlaySFX(sfxClip);

        if (effectPrefab == null) return;
        Instantiate(effectPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));

        // 다음 변경 시각 갱신
        nextChangeTime = Time.time + changeInterval;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!skillsInitialized)
        {
            if (playerAbility != null)
            {
                InitRuntimeSkills();

                if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
                    RollInitialSkill();
            }
        }

        if (currentSkill == null)
        {
            Debug.LogWarning("[SK_RandomDraw] currentSkill이 없습니다. Execute를 무시합니다.");
            return;
        }

        currentSkill.Execute(origin, direction);
    }

    private void SendStateIfPossible()
    {
        if (playerAbility == null) return;
        if (runtimeSkills.Count == 0) return;
        if (currentSkill == null) return;

        int idx = runtimeSkills.IndexOf(currentSkill);
        if (idx < 0) return;

        P2PMessageSender.SendMessage(
            SkillMessageBuilder.BuildRandomDrawState(assignedSlot, playerAbility.playerNumber, idx)
        );
    }

    public void ApplyRemotePick(int pickIndex)
    {
        if (!skillsInitialized)
        {
            if (playerAbility != null) InitRuntimeSkills();
        }

        if (runtimeSkills.Count == 0) return;
        if (pickIndex < 0 || pickIndex >= runtimeSkills.Count) return;

        currentSkill = runtimeSkills[pickIndex];

        if (currentSkill != null)
        {
            coolTime = Mathf.Max(0f, currentSkill.coolTime);
            skillIcon = currentSkill.skillIcon;
        }

        if (playerAbility != null)
        {
            playerAbility.CancelCooldown(assignedSlot);
            playerAbility.NotifySkillUIChanged(assignedSlot);
        }

        nextChangeTime = Time.time + changeInterval;
    }
}