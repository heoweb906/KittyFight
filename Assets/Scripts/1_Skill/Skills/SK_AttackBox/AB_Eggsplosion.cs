using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AB_Eggsplosion : AB_HitboxBase
{
    [Header("Blind")]
    [Min(0.01f)] public float blindnessDuration = 2.5f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var blind = victim.GetComponent<BlindStatus>();
        if (!blind) blind = victim.gameObject.AddComponent<BlindStatus>();

        blind.ApplyBlind(blindnessDuration);
    }
}