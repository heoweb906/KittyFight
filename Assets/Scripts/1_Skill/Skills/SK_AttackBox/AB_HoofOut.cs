using UnityEngine;

public class AB_HoofOut : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 40;
    public float knockbackForce = 10f;
    public float disableControlSeconds = 0.5f;

    [Header("넉백 비율")]
    public float lateralFactor = 1f; // 좌/우 성분 가중치
    public float upwardFactor = 1f; // 상향 성분 가중치

    // SK에서 넘겨주는 좌/우 부호(+1: 오른쪽, -1: 왼쪽)
    private float lateralSign = 1f;

    public void SetLateralSign(float sign)
    {
        lateralSign = sign >= 0f ? 1f : -1f;
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // 넉백: 스폰 시 확정된 좌/우 부호만 사용(오너 위치 참조 안함)
        var rb = victim.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 kbDir = (Vector3.right * lateralSign * lateralFactor + Vector3.up * upwardFactor).normalized;

            rb.velocity = Vector3.zero;
            rb.AddForce(kbDir * knockbackForce, ForceMode.Impulse);
        }

        // 컨트롤 차단 (피격자 쪽에서만 실행됨: AB_HitboxBase 권위 규칙)
        var controller = victim.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
            StartCoroutine(ReenableAfter(controller, disableControlSeconds));
        }

        // 데미지
        victim.TakeDamage(damage);
    }

    private System.Collections.IEnumerator ReenableAfter(MonoBehaviour m, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m != null) m.enabled = true;
    }
}