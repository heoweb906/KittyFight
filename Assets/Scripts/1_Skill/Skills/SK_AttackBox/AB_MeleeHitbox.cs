using UnityEngine;

public class AB_MeleeHitbox : AB_HitboxBase
{
    public int damage = 4;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage);
    }
}