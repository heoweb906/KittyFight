using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_SavageRoar : AB_HitboxBase
{
    public GameObject effect;

    [Header("스턴 수치")]
    public float stunDuration = 2.5f;


    private void Start()
    {
        if(effect) effect.transform.SetParent(null);
    }


    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();

        stun.ApplyStun(stunDuration);
    }
}
