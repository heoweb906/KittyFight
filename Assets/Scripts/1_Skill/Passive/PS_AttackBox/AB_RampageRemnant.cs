using UnityEngine;

public class AB_RampageRemnant : AB_HitboxBase
{
    [Header("Æø¹ß ÇÇÇØ")]
    public int damageToOthers = 20;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damageToOthers, ownerAbility);
    }
}
