using UnityEngine;

public class PS_HorsePower : Passive
{
    [Header("이동 속도 버프 설정")]
    [Tooltip("버프 지속 시간 (초)")]
    public float buffDuration = 3f;

    [Tooltip("이동 속도 증가 비율 (0.3 = 30% 증가)")]
    [Range(0f, 2f)]
    public float moveSpeedBonusRate = 0.3f;

    private float baseMoveSpeed;
    private bool baseSpeedInitialized = false;

    private float remainingBuffTime = 0f;

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

        remainingBuffTime = buffDuration;

        ApplyMoveSpeedBuff();
    }

    private void OnTick(float dt)
    {
        if (remainingBuffTime <= 0f) return;

        remainingBuffTime -= dt;
        if (remainingBuffTime <= 0f)
        {
            remainingBuffTime = 0f;
            RestoreBaseMoveSpeed();
        }
    }

    private void ApplyMoveSpeedBuff()
    {
        if (ability == null || !baseSpeedInitialized) return;

        float multiplier = 1f + moveSpeedBonusRate; // 30% 증가
        ability.moveSpeed = baseMoveSpeed * multiplier;
    }

    private void RestoreBaseMoveSpeed()
    {
        if (ability == null || !baseSpeedInitialized) return;

        ability.moveSpeed = baseMoveSpeed;
    }
}