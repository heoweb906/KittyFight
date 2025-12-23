using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using RayFire;
public enum WallCheckMode
{
    None,
    FromAttacker,
    FromHitbox
}


public abstract class AB_HitboxBase : MonoBehaviour
{
    [Header("공통 데이터")]
    [Tooltip("소유자 Ability(생성자)")]
    protected PlayerAbility ownerAbility;

    [Tooltip("한 번만 맞게 할지")]
    [SerializeField] protected bool singleHit = true;

    [Tooltip("수명(초). 0 이하이면 자동 파괴하지 않음")]
    [SerializeField] public float lifeTime = 0.2f;

    [Header("환경 충돌 설정")]
    [Tooltip("환경으로 간주할 레이어(벽/바닥 등)")]
    [SerializeField] protected LayerMask environmentMask;
    [SerializeField] protected WallCheckMode wallCheckMode = WallCheckMode.None;

    public bool bMiddleState = false;  // 중립 오브젝트 

    // 이미 맞춘 대상(중복 히트 방지)
    private readonly HashSet<PlayerHealth> _hitOnce = new HashSet<PlayerHealth>();


    protected virtual void Awake()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);

        StartEffect();
    }

    /// <summary>
    /// 스폰 직후 소유자 주입 (필수)
    /// </summary>
    public virtual void Init(PlayerAbility owner)
    {
        ownerAbility = owner;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        TryApplyHit(other);

        if (IsEnvironment(other.gameObject.layer))
            OnEnvironmentHit(other);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollision이 작동함");

        var col = collision.collider;

        // 1) 플레이어 피격 시도
        if (TryApplyHit(col)) return;

        Debug.Log("여기까지 잘 작동하고 있음");

        // 2) 환경 충돌 처리
        if (IsEnvironment(col.gameObject.layer))
            OnEnvironmentHit(col);
    }

    private bool TryApplyHit(Collider other)
    {
        if (!bMiddleState && ownerAbility == null) return false;

        // 2. 피격자 탐색
        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        var victimAbility = other.GetComponentInParent<PlayerAbility>();

        if (victimHealth == null || victimAbility == null) return false;

        // [수정] victimPN 선언을 위로 올림 (공통 사용)
        int victimPN = victimAbility.playerNumber;

        // 3. 중립 상태가 아닐 때만 '아군/자신 무시' 로직 수행
        if (!bMiddleState)
        {
            // ownerAbility는 위에서 null 체크가 끝났으므로 안전함
            int ownerPN = ownerAbility.playerNumber;

            if (victimPN == ownerPN) return false;
        }

        // 4. 중복 히트 방지
        if (singleHit && _hitOnce.Contains(victimHealth)) return false;
        _hitOnce.Add(victimHealth);

        // 5. 벽 체크
        if (IsBlockedByWall(other)) return false;

        // 6. 효과 적용 (여기서 victimPN 사용 가능)
        if (victimPN == MatchResultStore.myPlayerNumber)
        {
            ApplyEffects(victimHealth, other);
        }
        else
        {
            OnRemoteHit(victimHealth, other);
        }

        return true;










        //// 소유자 미세팅이면 판정 불가 
        //if (!bMiddleState && ownerAbility == null) return false;

        //// 피격자 탐색(자식 콜라이더 대응)
        //var victimHealth = other.GetComponentInParent<PlayerHealth>();
        //var victimAbility = other.GetComponentInParent<PlayerAbility>();

        //if (victimHealth == null || victimAbility == null) return false;

        //int ownerPN = ownerAbility.playerNumber;
        //int victimPN = victimAbility.playerNumber;

        //// 아군/자기 자신 무시
        //if (victimPN == ownerPN) return false;

        //// 중복 히트 방지
        //if (singleHit && _hitOnce.Contains(victimHealth)) return false;
        //_hitOnce.Add(victimHealth);

        //if (IsBlockedByWall(other)) return false;

        //if (victimPN == MatchResultStore.myPlayerNumber)
        //{
        //    // 실제 효과 적용
        //    ApplyEffects(victimHealth, other);
        //}
        //else
        //{
        //    OnRemoteHit(victimHealth, other);
        //}

        //return true;
    }

    protected bool IsEnvironment(int otherLayer)
    {
        return (environmentMask.value & (1 << otherLayer)) != 0;
    }

    private bool IsBlockedByWall(Collider victimCol)
    {
        if (wallCheckMode == WallCheckMode.None)
            return false;

        Vector3 origin;

        if (wallCheckMode == WallCheckMode.FromAttacker)
        {
            // 사용자 중심 기준
            origin = ownerAbility.transform.position;
        }
        else
        {
            // 설치형 기준
            origin = transform.position;
        }

        Vector3 target = victimCol.bounds.center;

        origin.z = 0f;
        target.z = 0f;

        return Physics.Linecast(
            origin,
            target,
            environmentMask,
            QueryTriggerInteraction.Ignore
        );
    }


    protected virtual void StartEffect() { }




    public void Explode(float range, float force)
    {
        Vector3 explosionPos = transform.position;

        // 1. OverlapSphere 대신 자식들에서 FragmentPiece 컴포넌트를 모두 찾습니다.
        FragmentPiece[] pieces = GetComponentsInChildren<FragmentPiece>();

        foreach (FragmentPiece piece in pieces)
        {
            // 2. 부모(자기 자신)는 건너뜁니다.
            if (piece.transform == transform) continue;

            // 3. 자식이 폭발 범위(range) 내에 있는지 확인합니다.
            float distance = Vector3.Distance(piece.transform.position, explosionPos);
            if (distance <= range)
            {
                Debug.Log("조각 발견");
                piece.OnThisPiece();

                // 4. Rigidbody를 찾아 힘을 가합니다.
                Rigidbody rb = piece.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (piece.transform.position - explosionPos).normalized;

                    // 거리가 0이면 방향이 (0,0,0)이 되어 문제가 생길 수 있으므로 기본 방향을 줍니다.
                    if (direction == Vector3.zero)
                    {
                        direction = Vector3.up; // 혹은 랜덤 방향
                    }

                    rb.AddForce(direction * force, ForceMode.Impulse);
                }
            }
        }
    }


    /// <summary>
    /// 환경(벽/바닥 등)과의 접촉 시 동작. 기본은 아무것도 안 함.
    /// 원거리 투사체 등에서 필요 시 오버라이드.
    /// </summary>
    protected virtual void OnEnvironmentHit(Collider other) { }

    /// <summary>
    /// 실제 효과 구현부: 파생 클래스에서 효과를 정의
    /// </summary>
    protected abstract void ApplyEffects(PlayerHealth victim, Collider victimCollider);

    // 사실상 BouncingFire 용
    protected virtual void OnRemoteHit(PlayerHealth victim, Collider victimCollider) { }
}