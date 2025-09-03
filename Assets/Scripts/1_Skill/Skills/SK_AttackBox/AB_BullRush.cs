using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AB_BullRush : AB_HitboxBase
{
    [Header("피해 (정수 ×10 스케일)")]
    public int damage = 35;

    [Header("넉백")]
    public float knockbackForce = 24f;
    public float upwardFactor = 0.4f;

    private Vector3 rushDir = Vector3.forward;

    public void SetRushDirection(Vector3 dir)
    {
        rushDir = new Vector3(dir.x, 0f, dir.z);
        if (rushDir.sqrMagnitude < 1e-6f) rushDir = transform.forward;
        rushDir.Normalize();
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // 피해
        victim.TakeDamage(damage, ownerAbility);

        // 넉백: 돌진 방향 + 상향 성분
        var rb = victim.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 kb = (rushDir + Vector3.up * upwardFactor).normalized;
            rb.velocity = Vector3.zero;
            rb.AddForce(kb * knockbackForce, ForceMode.Impulse);
        }
    }
}