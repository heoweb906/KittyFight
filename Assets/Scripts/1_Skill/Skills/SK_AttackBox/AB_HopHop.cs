using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_HopHop : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 30;

    [Header("Effect")]
    [SerializeField] private Transform effectRoot;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility);
    }

    protected override void Awake()
    {
        base.Awake();

        if (effectRoot != null)
        {
            effectRoot.SetParent(null, true);
        }
    }
}
