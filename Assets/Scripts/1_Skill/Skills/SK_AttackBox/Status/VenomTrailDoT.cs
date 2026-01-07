using System.Collections;
using UnityEngine;

public class VenomTrailDoT : MonoBehaviour
{
    private Coroutine poisonRoutine;

    public void ApplyPoison(float duration, float interval, int damage)
    {
        if (poisonRoutine != null)
        {
            StopCoroutine(poisonRoutine);
        }

        poisonRoutine = StartCoroutine(DoPoison(duration, interval, damage));
    }

    private IEnumerator DoPoison(float duration, float interval, int damage)
    {
        float elapsed = 0f;
        PlayerHealth health = GetComponent<PlayerHealth>();

        while (elapsed < duration)
        {
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        if(this != null) Destroy(this);
    }
}