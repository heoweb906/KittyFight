using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;
    private bool hasHit = false;
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            hasHit = true;
        }
    }
}