using UnityEngine;

public class PS_HotPotato : Passive
{
    [Header("투사체 프리팹")]
    public GameObject hotProjectilePrefab;

    [Header("핫 포테이토 스펙")]
    [Tooltip("원거리 쿨타임")]
    public float hotCooldown = 5f;

    private Skill rangedSkill;
    private GameObject originalProjectilePrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);

        if (ability != null)
        {
            rangedSkill = ability.GetSkill(SkillType.Ranged);
        }

        if (rangedSkill != null && hotProjectilePrefab != null)
        {
            originalProjectilePrefab = rangedSkill.objSkillEntity;
            rangedSkill.objSkillEntity = hotProjectilePrefab;
        }

        e.OnModifyCooldown += OnModifyCooldown;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;

        // 패시브 해제 시, 원래 원거리 투사체로 복원
        if (rangedSkill != null && originalProjectilePrefab != null)
        {
            rangedSkill.objSkillEntity = originalProjectilePrefab;
        }

        base.Unsubscribe(e);
    }

    private void OnModifyCooldown(SkillType slot, ref float seconds)
    {
        if (slot != SkillType.Ranged) return;
        seconds = hotCooldown;
    }
}