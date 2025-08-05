using UnityEngine;
using System.Collections;

public class AB_HoofOut : MonoBehaviour
{
    public int damage = 4;
    public float knockbackForce = 10f;
    public int ownerPlayerNumber;

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        //if (hasHit) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null || health.playerNumber == ownerPlayerNumber) return;
        //if (health.playerNumber != MatchResultStore.myPlayerNumber) return;

        hasHit = true;

        // 넉백
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }

        // 조작 차단
        MonoBehaviour controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
            StartCoroutine(EnableControlAfterDelay(controller, 0.5f));
        }

        health.TakeDamage(damage);
    }

    private IEnumerator EnableControlAfterDelay(MonoBehaviour controller, float delay)
    {
        yield return new WaitForSeconds(delay);
        controller.enabled = true;
    }
}
