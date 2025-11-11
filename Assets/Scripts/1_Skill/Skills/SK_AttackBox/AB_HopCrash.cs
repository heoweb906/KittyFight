using UnityEngine;

public class AB_HopCrash : AB_HitboxBase
{
    [Header("피해")]
    public int damage = 45; // SK에서 최종 계산값을 주입

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility, transform.position);
    }
}