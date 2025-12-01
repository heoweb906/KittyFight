using UnityEngine;

public class PS_Unbridled : Passive
{
    [Header("이속 증가 설정")]
    [Tooltip("최대 이속 증가 비율 (1.0 = +100% = 2배 이속)")]
    [Range(0f, 2f)]
    public float maxBonus = 1.0f;

    [Tooltip("초당 이속 증가 비율 (0.1 = 초당 +10%)")]
    public float bonusPerSecond = 0.1f;

    private float baseMoveSpeed;
    private float currentBonus;
    private bool initializedBaseSpeed = false;

    public override void OnEquip(PlayerAbility a)
    {
        base.OnEquip(a);

        if (ability != null)
        {
            baseMoveSpeed = ability.moveSpeed;
            initializedBaseSpeed = true;
        }
    }

    public override void OnUnequip()
    {
        ResetBonus(true);
        base.OnUnequip();
    }

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        e.OnBeforeTakeDamage += OnBeforeTakeDamage;
        e.OnRoundStart += OnRoundStart;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnBeforeTakeDamage -= OnBeforeTakeDamage;
        e.OnRoundStart -= OnRoundStart;
    }

    private void OnTick(float dt)
    {
        if (ability == null) return;

        if (!initializedBaseSpeed)
        {
            baseMoveSpeed = ability.moveSpeed;
            initializedBaseSpeed = true;
        }

        currentBonus += bonusPerSecond * dt;
        currentBonus = Mathf.Clamp(currentBonus, 0f, maxBonus);

        ApplyMoveSpeed();
    }

    private void OnBeforeTakeDamage(ref int dmg, GameObject attacker)
    {
        if (dmg <= 0) return;
        ResetBonus(true);
    }

    private void OnRoundStart(int roundIndex)
    {
        ResetBonus(true);
    }

    private void ApplyMoveSpeed()
    {
        if (ability == null || !initializedBaseSpeed) return;

        float multiplier = 1f + currentBonus;
        ability.moveSpeed = baseMoveSpeed * multiplier;
    }

    private void ResetBonus(bool resetSpeedImmediately)
    {
        currentBonus = 0f;

        if (resetSpeedImmediately && ability != null && initializedBaseSpeed)
        {
            ability.moveSpeed = baseMoveSpeed;
        }
    }
}