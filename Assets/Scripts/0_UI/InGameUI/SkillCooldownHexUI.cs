using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 육각 스킬 쿨타임바 (Filled 버전):
/// - Fill: Image Type=Filled, Fill Method=Vertical, Origin=Bottom 으로 설정
/// - 코드에서는 fillAmount(0~1)만 갱신
/// </summary>
public class SkillCooldownHexUI : MonoBehaviour
{
    [Header("표시 대상")]
    public PlayerAbility abilityRef;
    public SkillType slot;

    [Header("채움 이미지 (반드시 Filled/Vertical/Bottom)")]
    [SerializeField] private Image bgImage;
    [SerializeField] private Image fillImage;

    [Tooltip("True면 (1 - 남은비율)로 '충전차오름'처럼 표시")]
    public bool useChargeProgress = true;

    [Tooltip("0 = 즉시 반영, 값이 크면 부드럽게 보간")]
    [SerializeField, Range(0f, 30f)] private float lerpSpeed = 0f;

    [Tooltip("이 값 이상이면 '가득 참'으로 간주하여 애니메이션 1회 재생")]
    [SerializeField, Range(0.90f, 1.00f)] private float fullThreshold = 0.999f;

    [Tooltip("스킬 애니메이션")]
    [SerializeField] public SkillEffectAnimation effect;

    [Header("아이콘 표시 알파")]
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
        // 이전 구독 해제
        UnsubscribeAbility();

        abilityRef = ability;
        slot = newSlot;

        // 새 구독 + 즉시 갱신
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
        ApplyIconFromSkillIfAny(); // 같은 슬롯이면 아이콘 갱신
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
        var st = abilityRef.GetCooldown(slot); // st.Normalized = '남은 비율'(0~1)
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
                int idx = SlotToIndex(slot);   // 여기서 슬롯→인덱스 계산
                if (idx >= 0) effect.PlaySimpleAnimation(idx); // 해당 인덱스만 애니
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
            Debug.LogWarning($"[SkillUI] slot={slot} → abilityRef == null");
            return;
        }

        if (fillImage == null)
        {
            Debug.LogWarning($"[SkillUI] slot={slot} → fillImage == null");
            return;
        }

        var s = abilityRef.GetSkill(slot);
        var c = fillImage.color;

        if (s != null && s.skillIcon != null)
        {
            if (bgImage != null)
            {
                bgImage.sprite = s.skillIcon;
                bgImage.color = new Color32(25, 25, 25, 255);
            }

            fillImage.sprite = s.skillIcon;
            c.a = visibleAlphaWhenIcon;
            fillImage.color = c;
            fillImage.enabled = true;
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
    /// 인스펙터에서 잘못 세팅해도 런타임에 보정
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