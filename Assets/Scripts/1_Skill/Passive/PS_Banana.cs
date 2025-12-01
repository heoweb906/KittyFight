using UnityEngine;

public class PS_Banana : Passive
{
    [Header("바나나 투사체 프리팹")]
    [Tooltip("AB_Banana가 붙어 있는 바나나 투사체 프리팹")]
    public GameObject bananaProjectilePrefab;

    private Skill rangedSkill;
    private GameObject originalProjectilePrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        if (ability != null)
        {
            rangedSkill = ability.GetSkill(SkillType.Ranged);
        }

        if (rangedSkill != null && bananaProjectilePrefab != null)
        {
            originalProjectilePrefab = rangedSkill.objSkillEntity;
            rangedSkill.objSkillEntity = bananaProjectilePrefab;
        }
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        if (rangedSkill != null && originalProjectilePrefab != null)
        {
            rangedSkill.objSkillEntity = originalProjectilePrefab;
        }
    }
}