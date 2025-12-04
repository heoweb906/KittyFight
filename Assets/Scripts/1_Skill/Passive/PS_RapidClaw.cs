using UnityEngine;

public class PS_RapidClaw : Passive
{
    [Header("근접 쿨타임 감소량")]
    [Tooltip("근접 스킬(Melee)의 쿨타임에서 줄일 시간(초)")]
    public float reduceSeconds = 1f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        Instantiate(
            effectPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );
        // 쿨타임 확정 직전에 개입하는 훅
        e.OnModifyCooldown += OnModifyCooldown;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;
    }

    private void OnModifyCooldown(SkillType type, ref float duration)
    {
        if (type != SkillType.Melee) return;
        if (reduceSeconds <= 0f) return;
        if (duration <= 0f) return;

        duration = Mathf.Max(0f, duration - reduceSeconds);
    }
}