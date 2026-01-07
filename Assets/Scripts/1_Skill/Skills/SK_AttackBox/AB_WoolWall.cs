using UnityEngine;

public class AB_WoolWall : AB_HitboxBase
{
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private GameObject stunEffectPrefab;

    [Header("»ç¿îµå")]
    [SerializeField] private AudioClip destroySfxAudio;

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

        if (this)
        {
            ownerAbility?.PlaySFX(destroySfxAudio);
            Destroy(gameObject);
        }
    }
    protected override void OnRemoteHit(PlayerHealth victim, Collider victimCollider)
    {
        if (this)
        {
            ownerAbility?.PlaySFX(destroySfxAudio);
            Destroy(gameObject);
        }
    }
}