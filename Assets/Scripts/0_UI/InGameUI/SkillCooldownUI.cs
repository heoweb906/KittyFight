using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("표시 대상")]
    public PlayerAbility abilityRef;
    public SkillType slot;

    [Header("UI")]
    public Image cooldownOverlay;

    private void Update()
    {
        if (abilityRef == null || cooldownOverlay == null) return;

        var st = abilityRef.GetCooldown(slot);

        if (st.active)
        {
            cooldownOverlay.enabled = true;
            cooldownOverlay.fillAmount = st.Normalized; // 남은 비율
        }
        else
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.enabled = false;
        }
    }
}