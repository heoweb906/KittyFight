using UnityEngine;

public class AB_ShearShock : AB_HitboxBase
{
    [Header("���� ��ġ")]
    public float stunDuration = 2f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();

        stun.ApplyStun(stunDuration);
    }
}