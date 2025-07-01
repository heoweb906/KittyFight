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

        // 화면 좌표 → 월드 좌표 변환 (z축만 카메라-플레이어 거리)
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mp);

        // 커서 방향으로 벡터 계산 (3D 대각선 포함)
        Vector3 dir = (worldClick - transform.position).normalized;
        if (dir == Vector3.zero)
            dir = transform.forward;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dir * dashDistance;

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

        // 쿨타임
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}