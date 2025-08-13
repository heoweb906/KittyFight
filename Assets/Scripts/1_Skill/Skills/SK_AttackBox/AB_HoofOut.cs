using UnityEngine;

public class AB_HoofOut : AB_HitboxBase
{
    public int damage = 4;
    public float knockbackForce = 10f;
    public float disableControlSeconds = 0.5f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // 넉백
        var rb = victim.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (victim.transform.position - transform.position).normalized;
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }

        // 컨트롤 차단
        var controller = victim.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
            StartCoroutine(ReenableAfter(controller, disableControlSeconds));
        }

        victim.TakeDamage(damage);
    }

    private System.Collections.IEnumerator ReenableAfter(MonoBehaviour m, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m != null) m.enabled = true;
    }
}