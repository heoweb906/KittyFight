using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int player;
    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        Destroy(gameObject);
        hasHit = true;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;
        if (health.playerNumber == player) return;

        if (health != null)
        {
            if (health.playerNumber != MatchResultStore.myPlayerNumber) return;
            health.TakeDamage(damage);
        }
    }
}
