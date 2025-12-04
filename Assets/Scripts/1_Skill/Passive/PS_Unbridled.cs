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

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("Effect Scale")]
    [Tooltip("충전 0일 때 스케일 배수")]
    public float minScaleMul = 1f;
    [Tooltip("최대 충전일 때 스케일 배수")]
    public float maxScaleMul = 2f;

    private Transform effectInstance;
    private Vector3 effectBaseScale = Vector3.one;

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
        if (effectPrefab != null)
        {
            var fx = Instantiate(
                effectPrefab,
                transform.position,
                Quaternion.Euler(-90, 0, 0),
                transform
            );

            effectInstance = fx.transform;
            effectBaseScale = effectInstance.localScale;
            effectInstance.localScale = effectBaseScale * minScaleMul;
        }

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
        UpdateEffectScale();
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

        if (effectInstance != null)
        {
            effectInstance.localScale = effectBaseScale * minScaleMul;
        }
    }

    private void UpdateEffectScale()
    {
        if (effectInstance == null) return;
        if (maxBonus <= 0f)
        {
            effectInstance.localScale = effectBaseScale * minScaleMul;
            return;
        }

        float t = Mathf.Clamp01(currentBonus / maxBonus);
        float scaleMul = Mathf.Lerp(minScaleMul, maxScaleMul, t);
        effectInstance.localScale = effectBaseScale * scaleMul;
    }
}