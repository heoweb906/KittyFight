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

    private void Awake()
    {
        coolTime = 1.0f;
        aimRange = 3.5f;
        if (playerAbility != null)
            rb = playerAbility.GetComponent<Rigidbody>();
        if (!rb) rb = GetComponentInParent<Rigidbody>();
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


        Vector3 startPos = rb.position;
        float maxDistance = aimRange;

        if (obstacleMask == 0) obstacleMask = LayerMask.GetMask("Ground");

        RaycastHit hit;
        if (Physics.BoxCast(startPos, boxHalfExtents, direction, out hit, Quaternion.identity, maxDistance, obstacleMask))
            maxDistance = hit.distance;

        Vector3 targetPos = startPos + direction * maxDistance;

        StartCoroutine(DashLerp(startPos, targetPos));
    }

    private IEnumerator DashLerp(Vector3 startPos, Vector3 targetPos)
    {
        bool origUseGravity = rb.useGravity;
        Vector3 origVel = rb.velocity;

        if (disableGravityDuringDash) rb.useGravity = false;
        rb.velocity = Vector3.zero;

        float elapsed = 0f;
        float dur = Mathf.Max(0.0001f, dashDuration);
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