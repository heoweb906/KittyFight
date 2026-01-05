using UnityEngine;

public class AB_RampageRemnant : AB_HitboxBase
{
    [Header("폭발 피해")]
    public int damageToOthers = 20;

    [Header("사운드")]
    public AudioClip sfxAudio;

    private void Start()
    {
        ownerAbility.PlaySFX(sfxAudio);
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damageToOthers, ownerAbility);
    }
}
