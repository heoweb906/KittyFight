using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 육각 HP바 (Filled 버전):
/// - BG: 고정 배경 이미지(선택)
/// - Fill: Image Type=Filled, Fill Method=Horizontal, Origin=Left 로 설정
/// - 코드에서는 fillAmount(0~1)만 갱신
/// </summary>
public class PlayerHealthHexUI : MonoBehaviour
{
    [Header("참조")]
    public PlayerAbility abilityRef;

    [Header("배경 (옵션)")]
    [SerializeField] private Image bgImage;

    [Header("채움 이미지 (반드시 Filled/Horizontal/Left)")]
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

    private void FixedUpdate()
    {
        if (healthRef == null)
        {
            if (abilityRef != null) Bind(abilityRef);
            return;
        }

        Debug.Log(healthRef.CurrentHP);
        Debug.Log(healthRef.MaxHP);
        int cur = healthRef.CurrentHP;
        int max = healthRef.MaxHP;
        float t = Mathf.Clamp01((float)cur / max);
        fillImage.fillAmount = t;
    }

    /// <summary>
    /// Ability를 연결하고 HP 변화 이벤트를 구독한다.
    /// </summary>
    public void Bind(PlayerAbility ability)
    {
        // 이전 구독 정리
        if (healthRef != null) healthRef.OnHPChanged -= OnHPChanged;

        abilityRef = ability;
        healthRef = abilityRef != null ? abilityRef.Health : null;

        if (fillImage == null || healthRef == null) return;

        // 초기 값 반영
        ApplyHP(healthRef.CurrentHP, healthRef.MaxHP);

        // 이벤트 구독
        healthRef.OnHPChanged += OnHPChanged;
    }

    private void OnHPChanged(int cur, int max) => ApplyHP(cur, max);

    /// <summary>
    /// HP 비율을 Image.fillAmount(0~1)로 반영
    /// </summary>
    private void ApplyHP(int cur, int max)
    {
        Debug.Log("PlayerHealthHexUI_ApplyHP");

        if (fillImage == null || max <= 0) return;

        float t = Mathf.Clamp01((float)cur / max);

    

        fillImage.fillAmount = t;
    }

    /// <summary>
    /// 인스펙터가 안 맞아도 런타임에 강제 보정
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