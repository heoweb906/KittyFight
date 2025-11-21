using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_SavageRoar : AB_HitboxBase
{
    [Header("스턴 수치")]
    public float stunDuration = 2.5f;
    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();

        stun.ApplyStun(stunDuration);
    }
}
