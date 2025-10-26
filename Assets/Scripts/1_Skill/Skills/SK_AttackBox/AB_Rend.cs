using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AB_Rend : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 35;
    [Header("이펙트")]
    public GameObject aobjEffect;

    protected override void StartEffect()
    {
        if (aobjEffect != null)
        {
            GameObject effect = Instantiate(aobjEffect, transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.Euler(-58, -90, 0);
            effect.transform.localScale = new Vector3(0.35f, 1f, 1f);
            effect.transform.SetParent(null);
        }
    }


    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility);

      
    }
}