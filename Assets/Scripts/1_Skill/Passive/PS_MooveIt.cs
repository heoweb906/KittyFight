using UnityEngine;

public class PS_MooveIt : Passive
{
    public override int PassiveId => 104;

    [Header("쿨타임 감소 설정")]
    [Tooltip("대쉬 쿨타임 감소 비율 (0.4 = 40% 감소)")]
    [Range(0f, 1f)]
    public float reductionRate = 0.4f;

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
        e.OnModifyCooldown += OnModifyCooldown;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;
    }

    private void OnModifyCooldown(SkillType slot, ref float seconds)
    {
        if (slot != SkillType.Dash) return;
        float factor = 1f - reductionRate;
        seconds *= factor;

    }
}