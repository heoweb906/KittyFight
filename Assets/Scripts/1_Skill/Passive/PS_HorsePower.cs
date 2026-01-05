using UnityEngine;
using System.Collections.Generic;

public class PS_HorsePower : Passive
{
    public override int PassiveId => 119;

    [Header("이동 속도 버프 설정")]
    [Tooltip("버프 지속 시간 (초)")]
    public float buffDuration = 3f;

    [Tooltip("이동 속도 증가 비율 (0.3 = 30% 증가)")]
    [Range(0f, 2f)]
    public float moveSpeedBonusRate = 0.3f;

    public int maxStacks = 10;

    private float baseMoveSpeed;
    private bool baseSpeedInitialized = false;

    private readonly List<float> stackExpireTimes = new List<float>();

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        e.OnSkillExecuted += OnSkillExecuted;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnSkillExecuted -= OnSkillExecuted;
    }

    private void OnSkillExecuted(SkillType slot)
    {
        if (ability == null) return;

        if (!baseSpeedInitialized)
        {
            baseMoveSpeed = ability.moveSpeed;
            baseSpeedInitialized = true;
        }

        if (maxStacks <= 0) maxStacks = 1;
        if (stackExpireTimes.Count < maxStacks)
        {
            stackExpireTimes.Add(Time.time + buffDuration);
        }
        else
        {   
            // 최대 스택이면 가장 빨리 끝나는 스택을 갱신
            int earliestIdx = 0;
            float earliest = stackExpireTimes[0];
            for (int i = 1; i < stackExpireTimes.Count; i++)
            {
                if (stackExpireTimes[i] < earliest)
                {
                    earliest = stackExpireTimes[i];
                    earliestIdx = i;
                }
            }
            stackExpireTimes[earliestIdx] = Time.time + buffDuration;
        }

        if (effectPrefab)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }

        ApplyMoveSpeedBuff();
    }

    private void OnTick(float dt)
    {
        if (stackExpireTimes.Count == 0) return;

        float now = Time.time;

        for (int i = stackExpireTimes.Count - 1; i >= 0; i--)
        {
            if (stackExpireTimes[i] <= now)
                stackExpireTimes.RemoveAt(i);
        }

        if (stackExpireTimes.Count == 0)
        {
            RestoreBaseMoveSpeed();
            return;
        }

        ApplyMoveSpeedBuff();
    }

    private void ApplyMoveSpeedBuff()
    {
        if (ability == null || !baseSpeedInitialized) return;

        int stacks = stackExpireTimes.Count;

        float multiplier = 1f + moveSpeedBonusRate * stacks;
        ability.moveSpeed = baseMoveSpeed * multiplier;

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    private void RestoreBaseMoveSpeed()
    {
        if (ability == null || !baseSpeedInitialized) return;
        ability.moveSpeed = baseMoveSpeed;
    }
}