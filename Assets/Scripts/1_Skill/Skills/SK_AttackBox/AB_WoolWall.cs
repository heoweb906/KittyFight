using UnityEngine;

public class AB_WoolWall : AB_HitboxBase
{
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private GameObject stunEffectPrefab;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();
        stun.ApplyStun(stunDuration);

        Instantiate(
            stunEffectPrefab,
            victim.transform.position,
            Quaternion.identity,
            victim.transform
        );
    }
}