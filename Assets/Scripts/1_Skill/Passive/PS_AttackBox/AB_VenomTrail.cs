using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AB_VenomTrail : AB_HitboxBase
{
    [Header("독(DoT) 파라미터")]
    public float maxDurationInCloud = 1f;
    public float tickInterval = 1f;
    public int damagePerTick = 1;

    protected override void Awake()
    {
        singleHit = false;
        base.Awake();
    }

    private void RefreshPoison(PlayerHealth victim)
    {
        if (victim == null) return;

        var venom = victim.GetComponent<VenomTrailDoT>();
        if (!venom)
            venom = victim.gameObject.AddComponent<VenomTrailDoT>();

        venom.ApplyPoison(maxDurationInCloud, tickInterval, damagePerTick);
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        RefreshPoison(victim);
    }

    private void OnTriggerStay(Collider other)
    {
        if (ownerAbility == null) return;

        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        var victimAbility = other.GetComponentInParent<PlayerAbility>();
        if (victimHealth == null || victimAbility == null) return;

        int ownerPN = ownerAbility.playerNumber;
        int victimPN = victimAbility.playerNumber;

        if (victimPN == ownerPN) return;
        if (victimPN != MatchResultStore.myPlayerNumber) return;

        RefreshPoison(victimHealth);
    }

    private void OnTriggerExit(Collider other)
    {
        if (ownerAbility == null) return;

        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        var victimAbility = other.GetComponentInParent<PlayerAbility>();
        if (victimHealth == null || victimAbility == null) return;

        int ownerPN = ownerAbility.playerNumber;
        int victimPN = victimAbility.playerNumber;

        if (victimPN == ownerPN) return;
        if (victimPN != MatchResultStore.myPlayerNumber) return;

        var venom = victimHealth.GetComponent<VenomTrailDoT>();
        if (venom != null)
        {
            Destroy(venom);
        }
    }
}