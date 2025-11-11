using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AB_MoossiveQuake : AB_HitboxBase
{
    [Header("스턴 수치")]
    public float stunDuration = 3f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var anim = victim.GetComponentInChildren<Animator>();
        bool onGround = anim ? anim.GetBool("isGround") : false;
        bool onWall = anim ? anim.GetBool("isHanging") : false;

        if (!(onGround || onWall)) return;

        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();
        stun.ApplyStun(stunDuration);
    }
}