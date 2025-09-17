using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� HP�� (Filled ����):
/// - BG: ���� ��� �̹���(����)
/// - Fill: Image Type=Filled, Fill Method=Horizontal, Origin=Left �� ����
/// - �ڵ忡���� fillAmount(0~1)�� ����
/// </summary>
public class PlayerHealthHexUI : MonoBehaviour
{
    [Header("����")]
    public PlayerAbility abilityRef;

    [Header("��� (�ɼ�)")]
    [SerializeField] private Image bgImage;

    [Header("ä�� �̹��� (�ݵ�� Filled/Horizontal/Left)")]
    [SerializeField] private Image fillImage;

    private PlayerHealth healthRef;

    private void Awake()
    {
        EnsureFilledSetup();
    }

    private void OnEnable()
    {
        EnsureFilledSetup();
        if (abilityRef != null) Bind(abilityRef);
    }

    private void OnDisable()
    {
        if (healthRef != null) healthRef.OnHPChanged -= OnHPChanged;
    }

    private void OnDestroy()
    {
        if (healthRef != null) healthRef.OnHPChanged -= OnHPChanged;
    }

    /// <summary>
    /// Ability�� �����ϰ� HP ��ȭ �̺�Ʈ�� �����Ѵ�.
    /// </summary>
    public void Bind(PlayerAbility ability)
    {
        // ���� ���� ����
        if (healthRef != null) healthRef.OnHPChanged -= OnHPChanged;

        abilityRef = ability;
        healthRef = abilityRef != null ? abilityRef.Health : null;

        if (fillImage == null || healthRef == null) return;

        // �ʱ� �� �ݿ�
        ApplyHP(healthRef.CurrentHP, healthRef.MaxHP);

        // �̺�Ʈ ����
        healthRef.OnHPChanged += OnHPChanged;
    }

    private void OnHPChanged(int cur, int max) => ApplyHP(cur, max);

    /// <summary>
    /// HP ������ Image.fillAmount(0~1)�� �ݿ�
    /// </summary>
    private void ApplyHP(int cur, int max)
    {
        if (fillImage == null || max <= 0) return;

        float t = Mathf.Clamp01((float)cur / max);
        if (fillImage.type != Image.Type.Filled)
            fillImage.type = Image.Type.Filled;

        // ��������(�������� �� ������ ���� ���)
        if (fillImage.fillMethod != Image.FillMethod.Horizontal)
            fillImage.fillMethod = Image.FillMethod.Horizontal;

        if (fillImage.fillOrigin != (int)Image.OriginHorizontal.Left)
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        fillImage.fillAmount = t;
    }

    /// <summary>
    /// �ν����Ͱ� �� �¾Ƶ� ��Ÿ�ӿ� ���� ����
    /// </summary>
    private void EnsureFilledSetup()
    {
        if (fillImage == null) return;

        if (fillImage.type != Image.Type.Filled)
            fillImage.type = Image.Type.Filled;

        if (fillImage.fillMethod != Image.FillMethod.Horizontal)
            fillImage.fillMethod = Image.FillMethod.Horizontal;

        if (fillImage.fillOrigin != (int)Image.OriginHorizontal.Left)
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
    }
}