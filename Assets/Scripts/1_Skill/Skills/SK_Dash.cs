using UnityEngine;
using System.Collections;

public class SK_Dash : Skill
{
    [Header("Dash Params")]
    public float dashDuration = 0.08f;
    public Vector3 boxHalfExtents = new Vector3(0.4f, 0.4f, 0.4f);
    public LayerMask obstacleMask;                    // 충돌 레이어(비어있으면 Ground)

    public bool disableGravityDuringDash = true;      // 대쉬 중 중력 off

    private Rigidbody rb;

    [Header("패시브")]
    public AbilityEvents events;                      // Charge Rush 등에서 거리/속도 보정용

    [Header("카메라")]
    public float shakeAmount = 0.09f;

    private void Awake()
    {
        coolTime = 1.0f;
        aimRange = 3.5f;
        if (playerAbility != null)
            rb = playerAbility.GetComponent<Rigidbody>();
        if (!rb) rb = GetComponentInParent<Rigidbody>();
        if (playerAbility != null) events = playerAbility.events;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        // 로컬 소유자만 실제 이동 (원격은 쿨타임/UI만), 개발 확인 시에는 주석바람
        if (playerAbility == null || playerAbility.playerNumber != MatchResultStore.myPlayerNumber)
            return;

        if (!rb)
        {
            rb = playerAbility ? playerAbility.GetComponent<Rigidbody>() : GetComponentInParent<Rigidbody>();
            if (!rb) return;
        }

        if (direction.sqrMagnitude < 1e-6f) return;
        direction = direction.normalized;

        Vector3 startPos = rb.position;

        // 1) 기본 거리/속도 설정
        float baseDistance = aimRange;
        float baseSpeed = baseDistance / Mathf.Max(0.0001f, dashDuration);

        // 2) Charge Rush가 여기서 distance(1~3배), speed를 바꿔줌
        float desiredDistance = baseDistance;
        float desiredSpeed = baseSpeed;

        Debug.Log(desiredDistance);

        if (events != null)
        {
            Debug.Log("?");
            var p = new DashParams { distance = baseDistance, speed = baseSpeed };
            events.EmitDashWillExecute(ref p); // Charge Rush가 여기서 거리(및 속도) 변경
            desiredDistance = Mathf.Max(0f, p.distance);
            if (p.speed > 0f) desiredSpeed = p.speed; // 속도 보정도 지원(선택)
        }

        Debug.Log(desiredDistance);
        Debug.Log(desiredSpeed);

        if (obstacleMask == 0) obstacleMask = LayerMask.GetMask("Ground");

        float maxDistance = desiredDistance;
        RaycastHit hit;
        if (Physics.BoxCast(startPos, boxHalfExtents, direction, out hit, Quaternion.identity, maxDistance, obstacleMask))
            maxDistance = hit.distance;

        Vector3 targetPos = startPos + direction * maxDistance;

        float duration = dashDuration;
        if (events != null && desiredSpeed > 0f)
            duration = maxDistance / desiredSpeed;

        StartCoroutine(DashLerp(startPos, targetPos, duration));

        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCamera(shakeAmount, 0.2f);
        }
    }

    private IEnumerator DashLerp(Vector3 startPos, Vector3 targetPos, float duration)
    {
        bool origUseGravity = rb.useGravity;
        Vector3 origVel = rb.velocity;

        if (disableGravityDuringDash) rb.useGravity = false;
        rb.velocity = Vector3.zero;

        float elapsed = 0f;
        float dur = Mathf.Max(0.0001f, duration);
        while (elapsed < dur)
        {
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, elapsed / dur));
            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.MovePosition(targetPos);

        if (disableGravityDuringDash) rb.useGravity = origUseGravity;
        rb.velocity = Vector3.zero;
    }
}