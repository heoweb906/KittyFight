using UnityEngine;

public class AB_Ranged : AB_HitboxBase
{
    public int damage = 2;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage);
    }
}