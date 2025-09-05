using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    [Tooltip("��� ������")]
    public int lethalDamage = 10000;

    [Header("FX")]
    [SerializeField] private GameObject explodeEffectPrefab;

    private bool isInside = false;

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponentInParent<PlayerHealth>();
        var ability = other.GetComponentInParent<PlayerAbility>();
        if (health == null || ability == null) return;

        if (isInside) return;
        isInside = true;

        if (explodeEffectPrefab != null)
        {
            float xPos = transform.position.x;
            const float EPS = 1e-4f;

            Vector3 euler;
            if (Mathf.Abs(xPos) < EPS)        // ����
                euler = new Vector3(-90f, 90f, -90f);
            else if (xPos > 0f)               // ������
                euler = new Vector3(0f, -90f, -90f);
            else                               // ����
                euler = new Vector3(0f, 90f, -90f);

            Quaternion rot = Quaternion.Euler(euler);

            Instantiate(explodeEffectPrefab, other.transform.position, rot);
        }

        //  �� �÷��̾ ó��
        if (ability.playerNumber != MatchResultStore.myPlayerNumber) return;

        health.TakeDamage(lethalDamage, null);
    }

    private void OnTriggerExit(Collider other)
    {
        isInside = false;
    }
}