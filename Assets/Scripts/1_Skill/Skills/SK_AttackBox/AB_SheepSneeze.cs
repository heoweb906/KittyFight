using UnityEngine;

public class AB_SheepSneeze : AB_HitboxBase
{
    [Header("스턴 수치")]
    public float stunDuration = 1f;

    [Header("Effects")]
    [SerializeField] private GameObject stunEffectPrefab;
    public AudioClip sfxAudio;

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
    }
}