using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public Image cooldownOverlay;
    public float cooldownTime = 3f;

    private float remaining = 0f;

    public void StartCooldown()
    {
        remaining = cooldownTime;
        cooldownOverlay.fillAmount = 1f;
        cooldownOverlay.enabled = true;
    }

    private void Update()
    {
        if (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            cooldownOverlay.fillAmount = remaining / cooldownTime;

            if (remaining <= 0f)
            {
                cooldownOverlay.fillAmount = 0f;
                cooldownOverlay.enabled = false;
            }
        }
    }
}