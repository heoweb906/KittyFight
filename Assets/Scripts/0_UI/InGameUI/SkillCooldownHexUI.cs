using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ��ų ��Ÿ�ӹ� (Filled ����):
/// - Fill: Image Type=Filled, Fill Method=Vertical, Origin=Bottom ���� ����
/// - �ڵ忡���� fillAmount(0~1)�� ����
/// </summary>
public class SkillCooldownHexUI : MonoBehaviour
{
    [Header("ǥ�� ���")]
    public PlayerAbility abilityRef;
    public SkillType slot;

    [Header("ä�� �̹��� (�ݵ�� Filled/Vertical/Bottom)")]
    [SerializeField] private Image fillImage;

    [Tooltip("True�� (1 - ��������)�� '����������'ó�� ǥ��")]
    public bool useChargeProgress = true;

    [Tooltip("0 = ��� �ݿ�, ���� ũ�� �ε巴�� ����")]
    [SerializeField, Range(0f, 30f)] private float lerpSpeed = 0f;

    [Tooltip("�� �� �̻��̸� '���� ��'���� �����Ͽ� �ִϸ��̼� 1ȸ ���")]
    [SerializeField, Range(0.90f, 1.00f)] private float fullThreshold = 0.999f;

    [Tooltip("��ų �ִϸ��̼�")]
    [SerializeField] public SkillEffectAnimation effect;

    [Header("������ ǥ�� ����")]
    [SerializeField, Range(0f, 1f)] private float visibleAlphaWhenIcon = 1f;
    [SerializeField, Range(0f, 1f)] private float noIconAlphaWhenNone = 0f;

    private float currentFill;
    private bool hasPlayedReadyAnim;

    private void Awake()
    {
        EnsureFilledSetup();
    }

    private void OnEnable()
    {
        EnsureFilledSetup();
        currentFill = 0f;
        hasPlayedReadyAnim = false;

        if (abilityRef != null)
        {
            SubscribeAbility();
            ApplyIconFromSkillIfAny();
            ForceRefreshFillImmediate();
        }
        else
        {
            if (fillImage != null)
            {
                var c = fillImage.color; c.a = noIconAlphaWhenNone; fillImage.color = c;
                fillImage.sprite = null;
            }
        }
    }

    private void OnDisable()
    {
        UnsubscribeAbility();
    }

    public void Bind(PlayerAbility ability, SkillType newSlot)
    {
        // ���� ���� ����
        UnsubscribeAbility();

        abilityRef = ability;
        slot = newSlot;

        // �� ���� + ��� ����
        SubscribeAbility();
        ApplyIconFromSkillIfAny();
        ForceRefreshFillImmediate();

        Debug.Log($"[SkillUI] Bound -> ability={abilityRef?.name}, slot={slot}, ui={name}", this);
    }

    private void ForceRefreshFillImmediate()
    {
        if (abilityRef == null || fillImage == null) return;
        var t = CalcVisual();
        fillImage.fillAmount = t;
        currentFill = t;
        hasPlayedReadyAnim = (t >= fullThreshold);
    }

    private void SubscribeAbility()
    {
        if (abilityRef == null) return;
        abilityRef.OnSkillEquipped += HandleSkillEquipped;
    }

    private void UnsubscribeAbility()
    {
        if (abilityRef == null) return;
        abilityRef.OnSkillEquipped -= HandleSkillEquipped;
    }

    private void HandleSkillEquipped(SkillType equippedSlot, Skill skill)
    {
        if (equippedSlot != slot) return;
        ApplyIconFromSkillIfAny(); // ���� �����̸� ������ ����
    }

    private void Update()
    {
        if (abilityRef == null || fillImage == null) return;

        float target = CalcVisual(); // 0~1

        if (lerpSpeed > 0f)
        {
            currentFill = Mathf.MoveTowards(
                currentFill,
                target,
                lerpSpeed * Time.unscaledDeltaTime
            );
            fillImage.fillAmount = currentFill;
        }
        else
        {
            fillImage.fillAmount = target;
            currentFill = target;
        }

        HandleReadyAnimation(currentFill);
    }

    private float CalcVisual()
    {
        var st = abilityRef.GetCooldown(slot); // st.Normalized = '���� ����'(0~1)
        float remaining = Mathf.Clamp01(st.Normalized);
        return useChargeProgress ? (1f - remaining) : remaining; // 0~1
    }

    private void HandleReadyAnimation(float visual)
    {
        bool isFull = visual >= fullThreshold;

        if (isFull && !hasPlayedReadyAnim)
        {
            if (effect != null)
            {
                int idx = SlotToIndex(slot);   // ���⼭ ���ԡ��ε��� ���
                if (idx >= 0) effect.PlaySimpleAnimation(idx); // �ش� �ε����� �ִ�
            }
            hasPlayedReadyAnim = true;
        }

        if (!isFull && hasPlayedReadyAnim)
            hasPlayedReadyAnim = false;
    }

    private int SlotToIndex(SkillType s)
    {
        switch (s)
        {
            case SkillType.Melee: return 0;
            case SkillType.Ranged: return 1;
            case SkillType.Dash: return 2;
            case SkillType.Skill1: return 3;
            case SkillType.Skill2: return 4;
            default: return -1;
        }
    }

    private void ApplyIconFromSkillIfAny()
    {
        if (abilityRef == null)
        {
            Debug.LogWarning($"[SkillUI] slot={slot} �� abilityRef == null");
            return;
        }

        if (fillImage == null)
        {
            Debug.LogWarning($"[SkillUI] slot={slot} �� fillImage == null");
            return;
        }

        if (abilityRef == null || fillImage == null) return;
        var s = abilityRef.GetSkill(slot);
        var c = fillImage.color;
        if (s != null && s.skillIcon != null)
        {
            fillImage.sprite = s.skillIcon;
            c.a = visibleAlphaWhenIcon;
            fillImage.color = c;
            fillImage.enabled = true;
            // fillImage.preserveAspect = true;
        }
        else
        {
            fillImage.sprite = null;
            c.a = noIconAlphaWhenNone;
            fillImage.color = c;
            fillImage.enabled = true;
        }
    }

    /// <summary>
    /// �ν����Ϳ��� �߸� �����ص� ��Ÿ�ӿ� ����
    /// </summary>
    private void EnsureFilledSetup()
    {
        if (fillImage == null) return;

        if (fillImage.type != Image.Type.Filled)
            fillImage.type = Image.Type.Filled;

        if (fillImage.fillMethod != Image.FillMethod.Vertical)
            fillImage.fillMethod = Image.FillMethod.Vertical;

        if (fillImage.fillOrigin != (int)Image.OriginVertical.Bottom)
            fillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
    }
}