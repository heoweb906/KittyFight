using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 모든 근접 히트박스/투사체가 공통으로 갖는 판정/소유/친선무시/권위 체크 로직
/// </summary>
public abstract class AB_HitboxBase : MonoBehaviour
{
    [Header("공통 데이터")]
    [Tooltip("소유자 Ability(생성자)")]
    protected PlayerAbility ownerAbility;

    [Tooltip("한 번만 맞게 할지")]
    [SerializeField] protected bool singleHit = true;

    [Tooltip("수명(초). 0 이하이면 자동 파괴하지 않음")]
    [SerializeField] protected float lifeTime = 1.0f;

    // 이미 맞춘 대상(중복 히트 방지)
    private readonly HashSet<PlayerHealth> _hitOnce = new HashSet<PlayerHealth>();

    protected virtual void Awake()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// 스폰 직후 소유자 주입 (필수)
    /// </summary>
    public virtual void Init(PlayerAbility owner)
    {
        ownerAbility = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 피격자 컴포넌트 획득 (자식 콜라이더 대응)
        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        if (victimHealth == null) return;

        var victimAbility = other.GetComponentInParent<PlayerAbility>();
        if (victimAbility == null) return;

        // 소유자 미세팅이면 판정 불가 → 안전 탈출
        if (ownerAbility == null) return;

        int ownerPN = ownerAbility.playerNumber;
        int victimPN = victimAbility.playerNumber;

        // 아군/자기 자신 무시
        if (victimPN == ownerPN) return;

        // 이 클라이언트가 "맞은 당사자(=나)"일 때만 처리 (권위 일원화)
        if (victimPN != MatchResultStore.myPlayerNumber) return;

        // 중복 히트 방지
        if (singleHit && _hitOnce.Contains(victimHealth)) return;
        _hitOnce.Add(victimHealth);

        // 개별 효과 실행(데미지/넉백/상태 등)
        ApplyEffects(victimHealth, other);
    }

    /// <summary>
    /// 실제 효과 구현부: 파생 클래스에서 효과를 정의
    /// </summary>
    protected abstract void ApplyEffects(PlayerHealth victim, Collider victimCollider);
}