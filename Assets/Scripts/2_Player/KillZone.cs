using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    [Tooltip("즉사 데미지")]
    public int lethalDamage = 10000;

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponentInParent<PlayerHealth>();
        var ability = other.GetComponentInParent<PlayerAbility>();
        if (health == null || ability == null) return;

        //  내 플레이어만 처리
        if (ability.playerNumber != MatchResultStore.myPlayerNumber) return;

        health.TakeDamage(lethalDamage, null);
    }
}