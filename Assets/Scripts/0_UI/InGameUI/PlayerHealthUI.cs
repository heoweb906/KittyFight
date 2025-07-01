using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    //[SerializeField] private TMP_Text hpText;
    [SerializeField] private Image hpFillImage;

    private int maxHP;
    private int currentHP;

    public void Initialize(int max)
    {
        maxHP = max;
        currentHP = max;
        UpdateUI();
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateUI();
    }

    private void UpdateUI()
    {
        //if (hpText != null)
        //    hpText.text = $"{currentHP} / {maxHP}";

        if (hpFillImage != null)
            hpFillImage.fillAmount = (float)currentHP / maxHP;
    }
}