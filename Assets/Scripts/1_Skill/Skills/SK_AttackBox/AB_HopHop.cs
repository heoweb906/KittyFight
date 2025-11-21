using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_HopHop : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 30;
    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility);
    }
}
