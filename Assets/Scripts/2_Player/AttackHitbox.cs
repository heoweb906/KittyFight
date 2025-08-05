using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage;
    public int player;
    private bool hasHit = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;
        if (health.playerNumber == player) return;

        if (hasHit) return;
        hasHit = true;

        if (health != null)
        {
            if (health.playerNumber != MatchResultStore.myPlayerNumber) return; // 내가 상대를 맞췄을 경우에는 처리하지 않음
            health.TakeDamage(damage);
        }
    }
}