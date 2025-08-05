// PlayerDash.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerDash : MonoBehaviour
{
    public float dashDistance = 3f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 1f;
    public int myPlayerNumber;

    private Rigidbody rb;
    private bool canDash = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TryDash()
    {
        if (canDash)
            StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;

        // 마우스 방향 계산
        Vector3 aimPos, dir;
        AttackUtils.GetAimPointAndDirection(transform, dashDistance, out aimPos, out dir);

        Vector3 startPos = transform.position;
        float maxDistance = dashDistance;

        // BoxCast 충돌 검사
        RaycastHit hit;
        Vector3 boxHalfExtents = new Vector3(0.4f, 0.4f, 0.4f);
        Quaternion orientation = Quaternion.identity;
        LayerMask layerMask = LayerMask.GetMask("Ground"); // 충돌 레이어

        if (Physics.BoxCast(startPos, boxHalfExtents, dir, out hit, orientation, dashDistance, layerMask))
        {
            maxDistance = hit.distance;
        }

        Vector3 targetPos = startPos + dir * maxDistance;

        // 대쉬 중 중력 해제
        bool origGravity = rb.useGravity;
        rb.useGravity = false;

        // Lerp 이동
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, elapsed / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.MovePosition(targetPos);

        // 대쉬 완료 후 중력 복원
        rb.useGravity = origGravity;
        rb.velocity = Vector3.zero;

        // 쿨타임
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}