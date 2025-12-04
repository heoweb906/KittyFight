using UnityEngine;

public class PS_IQ20000 : Passive
{
    [Header("Cooldown")]
    [Tooltip("모든 스킬 쿨타임에서 뺄 초 단위 수치")]
    public float reduceBySeconds = 2f;

    private GameObject auraFxInstance;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        Instantiate(
            effectPrefab,
            transform.position,
            Quaternion.Euler(-90f, 0f, 0f),
            transform
        );
        // 쿨타임 계산 직전에 2초 감소
        e.OnModifyCooldown += HandleModifyCooldown;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= HandleModifyCooldown;
    }

    private void HandleModifyCooldown(SkillType slot, ref float seconds)
    {
        if (reduceBySeconds <= 0f) return;
        seconds = Mathf.Max(0f, seconds - reduceBySeconds);
    }
}