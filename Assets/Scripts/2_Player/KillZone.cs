using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    [Tooltip("��� ������")]
    public int lethalDamage = 10000;

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponentInParent<PlayerHealth>();
        var ability = other.GetComponentInParent<PlayerAbility>();
        if (health == null || ability == null) return;

        //  �� �÷��̾ ó��
        if (ability.playerNumber != MatchResultStore.myPlayerNumber) return;

        health.TakeDamage(lethalDamage, null);
    }
}