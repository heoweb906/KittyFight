using UnityEngine;

public class AB_BunnyBounce : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 15;
    public float knockbackForce = 8f;
    public float disableControlSeconds = 0.2f;

    [Header("넉백 튜닝")]
    [Tooltip("위쪽 성분 비율 (0이면 순수 수평, 1이면 위쪽 성분 강하게)")]
    public float upwardFactor = 0.5f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // 넉백
        var rb = victim.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 히트박스 중심 기준 바깥 방향 + 약간 위로
            Vector3 rawDir = victim.transform.position - transform.position;
            rawDir.z = 0f;

            if (rawDir.sqrMagnitude < 0.0001f)
            {
                rawDir = Vector3.up;
            }

            Vector3 kbDir = rawDir.normalized;
            kbDir += Vector3.up * upwardFactor;   // 약간 위쪽으로 보정
            kbDir = kbDir.normalized;

            rb.velocity = Vector3.zero;
            rb.AddForce(kbDir * knockbackForce, ForceMode.Impulse);
        }

        var pj = victim.GetComponent<PlayerJump>();
        if (pj != null)
        {
            pj.IgnoreHardStopFor(0.12f);
        }

        if (disableControlSeconds > 0f)
        {
            var controller = victim.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.DisableControlFor(disableControlSeconds);
            }
        }

        victim.TakeDamage(damage, ownerAbility, transform.position);
    }
}