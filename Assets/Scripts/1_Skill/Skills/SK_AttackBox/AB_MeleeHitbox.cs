using UnityEngine;

public class AB_MeleeHitbox : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 40;

    public GameObject objEffect;


    private void Start()
    {
        objEffect.transform.SetParent(null);
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
     

        victim.TakeDamage(damage, ownerAbility, transform.position);
    }
}