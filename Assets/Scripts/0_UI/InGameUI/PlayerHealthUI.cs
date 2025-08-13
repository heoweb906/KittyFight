using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerAbility abilityRef;
    [SerializeField] private Slider hpSlider;

    private PlayerHealth healthRef;

    public void Bind(PlayerAbility ability)
    {
        // ���� ���� ����
        if (healthRef != null) healthRef.OnHPChanged -= OnHPChanged;

        abilityRef = ability;
        healthRef = abilityRef != null ? abilityRef.Health : null;

        if (hpSlider == null || healthRef == null) return;

        hpSlider.minValue = 0;
        hpSlider.maxValue = healthRef.MaxHP;
        hpSlider.value = healthRef.CurrentHP;

        // �̺�Ʈ ����
        healthRef.OnHPChanged += OnHPChanged;
    }

    private void OnDisable()
    {
        if (healthRef != null) healthRef.OnHPChanged -= OnHPChanged;
    }

    private void OnHPChanged(int cur, int max)
    {
        if (hpSlider == null) return;
        if ((int)hpSlider.maxValue != max) hpSlider.maxValue = max;
        hpSlider.value = cur;
    }

    // �̺�Ʈ ���� ������ ��� ����(����)
    private void Update()
    {
        if (healthRef == null || hpSlider == null) return;

        if ((int)hpSlider.maxValue != healthRef.MaxHP)
            hpSlider.maxValue = healthRef.MaxHP;

        if ((int)hpSlider.value != healthRef.CurrentHP)
            hpSlider.value = healthRef.CurrentHP;
    }
}