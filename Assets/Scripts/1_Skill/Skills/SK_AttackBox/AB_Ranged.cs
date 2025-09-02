using UnityEngine;

public class AB_Ranged : AB_HitboxBase
{
    [Header("����/����")]
    public int damage = 20;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility);
    }
}