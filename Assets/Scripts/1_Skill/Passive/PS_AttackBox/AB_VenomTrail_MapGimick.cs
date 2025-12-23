using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AB_VenomTrail_MapGimick : AB_HitboxBase
{
    [Header("독(DoT) 파라미터")]
    public float maxDurationInCloud = 1f;
    public float tickInterval = 1f;
    public int damagePerTick = 1;

    protected override void Awake()
    {
        base.Awake();
        singleHit = false; // Awake 순서 중요 (base 호출 후 설정 혹은 전 설정)
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
        // [수정 1] 중립이 아니면서 오너가 없으면 리턴 (중립이면 통과)
        if (!bMiddleState && ownerAbility == null) return;

        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        var victimAbility = other.GetComponentInParent<PlayerAbility>();
        if (victimHealth == null || victimAbility == null) return;

        // [수정 2] 중립이 아닐 때만 아군 체크 수행
        if (!bMiddleState)
        {
            int ownerPN = ownerAbility.playerNumber;
            int victimPN = victimAbility.playerNumber;
            if (victimPN == ownerPN) return; // 아군 피격 방지
        }

        // 내 캐릭터인지 확인 (상대방 캐릭터는 동기화 문제로 여기서 처리 안 할 경우)
        if (victimAbility.playerNumber != MatchResultStore.myPlayerNumber) return;

        RefreshPoison(victimHealth);
    }

    private void OnTriggerExit(Collider other)
    {
        // [수정 1] 중립이 아니면서 오너가 없으면 리턴
        if (!bMiddleState && ownerAbility == null) return;

        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        var victimAbility = other.GetComponentInParent<PlayerAbility>();
        if (victimHealth == null || victimAbility == null) return;

        // [수정 2] 중립이 아닐 때만 아군 체크
        if (!bMiddleState)
        {
            int ownerPN = ownerAbility.playerNumber;
            int victimPN = victimAbility.playerNumber;
            if (victimPN == ownerPN) return;
        }

        if (victimAbility.playerNumber != MatchResultStore.myPlayerNumber) return;

        var venom = victimHealth.GetComponent<VenomTrailDoT>();
        if (venom != null)
        {
            Destroy(venom);
        }
    }
}