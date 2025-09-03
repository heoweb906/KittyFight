using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AB_FeatherShot : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 5;

    [Header("동작")]
    public bool destroyOnHit = true;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility);
        if (destroyOnHit && this) Destroy(gameObject);
    }

    protected override void OnEnvironmentHit(Collider other)
    {
        Destroy(gameObject);
    }
}