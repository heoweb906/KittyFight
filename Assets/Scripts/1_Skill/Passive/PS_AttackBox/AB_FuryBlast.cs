using UnityEngine;

public class AB_FuryBlast : AB_HitboxBase
{
    [Header("폭발 피해")]
    public int damageToOthers = 20;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // 주변 적에게 2 데미지
        victim.TakeDamage(damageToOthers, ownerAbility);
    }
}