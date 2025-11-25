using UnityEngine;

public class AB_WoolWall : AB_HitboxBase
{
    [SerializeField] private float stunDuration = 3f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();
        stun.ApplyStun(stunDuration);
    }
}