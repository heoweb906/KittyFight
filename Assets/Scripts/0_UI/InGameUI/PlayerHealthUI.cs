using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    //[SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider hpSlider;

    private int maxHP;
    private int currentHP;

    public void Initialize(int max)
    {
        maxHP = max;
        currentHP = max;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.minValue = 0;
            hpSlider.value = maxHP;
        }
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, 9);
        //currentHP = Mathf.Clamp(hp, 0, maxHP);
        if (hpSlider != null)
            hpSlider.value = currentHP;
    }
}