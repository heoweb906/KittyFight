using System.Collections;
using UnityEngine;

public class SlowStatus : MonoBehaviour
{
    private PlayerAbility ability;
    private Coroutine slowRoutine;

    private float originalMoveSpeed;
    private bool hasOriginal = false;

    private float currentMultiplier = 1f;

    private void Awake()
    {
        ability = GetComponent<PlayerAbility>();
    }

    public void ApplySlowPercent(float percent, float duration)
    {
        float clamped = Mathf.Clamp(percent, 0f, 100f);
        float multiplier = 1f - clamped / 100f;
        ApplySlowMultiplier(multiplier, duration);
    }

    public void ApplySlowMultiplier(float multiplier, float duration)
    {
        if (ability == null)
            ability = GetComponent<PlayerAbility>();
        if (ability == null) return;

        if (!hasOriginal)
        {
            originalMoveSpeed = ability.moveSpeed;
            hasOriginal = true;
        }

        currentMultiplier = Mathf.Clamp01(multiplier);

        ability.moveSpeed = originalMoveSpeed * currentMultiplier;

        if (slowRoutine != null)
            StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(SlowTimer(duration));
    }

    private IEnumerator SlowTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (ability != null && hasOriginal)
        {
            ability.moveSpeed = originalMoveSpeed;
        }

        Destroy(this);
    }
}