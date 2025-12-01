using UnityEngine;

public class AB_SheepSneeze : AB_HitboxBase
{
    [Header("스턴 수치")]
    public float stunDuration = 1f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();

        stun.ApplyStun(stunDuration);
    }
}