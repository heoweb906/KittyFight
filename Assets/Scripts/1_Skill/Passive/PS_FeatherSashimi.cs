using UnityEngine;

public class PS_FeatherSashimi : Passive
{
    public override int PassiveId => 130;

    [Header("Feather Sashimi 설정")]
    [Tooltip("깃털 근접 공격 이펙트 프리팹 (예: Slash VFX)")]
    public GameObject featherEffectPrefab;

    [Tooltip("새로운 근접 공격 쿨타임")]
    public float newCooldown = 1.0f;

    private Skill meleeSkill;
    private GameObject originalProjectilePrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        if (ability != null)
        {
            meleeSkill = ability.GetSkill(SkillType.Melee);
        }

        if (meleeSkill != null && featherEffectPrefab != null)
        {
            originalProjectilePrefab = meleeSkill.objSkillEntity;
            meleeSkill.aimRange = 2;
            meleeSkill.objSkillEntity = featherEffectPrefab;
        }

        e.OnModifyCooldown += OnModifyCooldown;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;
        if (meleeSkill != null && originalProjectilePrefab != null)
        {
            meleeSkill.objSkillEntity = originalProjectilePrefab;
        }
    }
    private void OnModifyCooldown(SkillType slot, ref float seconds)
    {
        if (slot != SkillType.Melee) return;
        seconds = newCooldown;
    }
}