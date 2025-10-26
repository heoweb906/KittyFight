using UnityEngine;

public class AB_ShearShock : AB_HitboxBase
{
    [Header("스턴 수치")]
    public float stunDuration = 2f;

    [Header("Effect")]
    public GameObject obj_Effect;


    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();

        stun.ApplyStun(stunDuration);
    }


    protected override void StartEffect()
    {

        if (obj_Effect != null)
        {
            obj_Effect.transform.SetParent(null);
        }

    }
}