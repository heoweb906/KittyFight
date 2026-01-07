using UnityEngine;

public class AB_SheepSneeze : AB_HitboxBase
{
    [Header("스턴 수치")]
    public float stunDuration = 1f;

    [Header("Effects")]
    [SerializeField] private GameObject stunEffectPrefab;
    public AudioClip sfxAudio;

    [Header("사운드")]
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

        ownerAbility.PlaySFX(sfxAudio);

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