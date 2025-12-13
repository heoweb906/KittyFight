using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Button_MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button buttton;
    public TMP_Text text_ButtonText;

    private Image targetImage;
    private Color originalImageColor;
    private Color originalTextColor;

    // 초기화 여부 체크 (Start보다 OnDisable이 먼저 실행되는 에러 방지)
    private bool isInitialized = false;

    void Start()
    {
        InitializeColors();
    }

    private void InitializeColors()
    {
        if (isInitialized) return;

        if (buttton != null)
        {
            targetImage = buttton.image;
            if (targetImage != null)
                originalImageColor = targetImage.color;
        }

        if (text_ButtonText != null)
        {
            originalTextColor = text_ButtonText.color;
        }

        isInitialized = true;
    }

    // ★ 핵심 추가: 오브젝트가 꺼질 때 강제로 상태 리셋
    private void OnDisable()
    {
        ResetButtonState();
    }

    // 상태를 초기화하는 함수 (외부에서도 호출 가능)
    public void ResetButtonState()
    {
        // 초기화가 안 된 상태면 색상 저장이 안 되어 있으므로 패스
        if (!isInitialized) return;

        if (targetImage != null)
        {
            targetImage.DOKill(); // 애니메이션 즉시 중단
            targetImage.color = originalImageColor; // 원래 색으로 즉시 복구
        }

        if (text_ButtonText != null)
        {
            text_ButtonText.DOKill();
            text_ButtonText.color = originalTextColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.DOKill();
            targetImage.DOColor(Color.white, 0.1f);
        }

        if (text_ButtonText != null)
        {
            text_ButtonText.DOKill();
            text_ButtonText.DOColor(Color.black, 0.1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.DOKill();
            targetImage.DOColor(originalImageColor, 0.1f);
        }

        if (text_ButtonText != null)
        {
            text_ButtonText.DOKill();
            text_ButtonText.DOColor(originalTextColor, 0.1f);
        }
    }
}