using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("ǥ�� ���")]
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
            cooldownOverlay.fillAmount = st.Normalized; // ���� ����
        }
        else
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.enabled = false;
        }
    }
}