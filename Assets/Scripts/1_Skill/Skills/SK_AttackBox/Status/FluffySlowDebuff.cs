using UnityEngine;
public class FluffySlowDebuff : MonoBehaviour
{
    private PlayerAbility ability;
    private float remainingTime = 0f;
    private float currentMultiplier = 1f;

    public void Apply(PlayerAbility targetAbility, float duration, float multiplier)
    {
        if (targetAbility == null) return;

        if (ability == null)
        {
            ability = targetAbility;
            currentMultiplier = 1f;
        }

        remainingTime = duration;

        float clampedMultiplier = Mathf.Clamp(multiplier, 0.01f, 1f);

        float currentSpeed = ability.moveSpeed;
        float logicalBaseSpeed = currentSpeed / currentMultiplier;

        currentMultiplier = clampedMultiplier;
        ability.moveSpeed = logicalBaseSpeed * currentMultiplier;
    }

    private void Update()
    {
        if (ability == null)
        {
            Destroy(this);
            return;
        }

        if (remainingTime <= 0f) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            float currentSpeed = ability.moveSpeed;
            float logicalBaseSpeed = currentSpeed / currentMultiplier;

            currentMultiplier = 1f;
            ability.moveSpeed = logicalBaseSpeed * currentMultiplier;

            Destroy(this);
        }
    }
}