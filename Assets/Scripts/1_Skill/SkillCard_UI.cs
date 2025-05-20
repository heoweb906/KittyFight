using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; 

public class SkillCard_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SkillCardController skillCardController { get; set; }
    public bool bCanInteract = false;

    [Header("UI 관련")]
    public Image image_BackGround;
    public Image image_SkillIcon;
    public TextMeshProUGUI text_SkillName;
    public TextMeshProUGUI text_SkilDescription;

    [Header("스킬")]
    public SkillCard_SO skillCard_SO;

    // 원본 값 보관용
    private Vector3 originalScale;
    private Color originalBGColor;

    // 트윈 설정
    private float tweenDuration = 0.08f;
    private float scaleFactor = 0.95f;



    private void Awake()
    {
        originalScale = transform.localScale;
        originalBGColor = image_BackGround.color;
    }






    /// <summary>
    /// SO 데이터 기반으로 UI 요소 초기화
    /// </summary>
    public void ApplyData(SkillCard_SO data)
    {
        if (data == null) return;

        skillCard_SO = data;

        // Background 이미지 설정
        if (data.image_BackGround != null && data.image_BackGround.sprite != null)
            image_BackGround.sprite = data.image_BackGround.sprite;

        // Skill Icon 설정
        if (data.image_SkillIcon != null && data.image_SkillIcon.sprite != null)
            image_SkillIcon.sprite = data.image_SkillIcon.sprite;

        // 텍스트 설정
        text_SkillName.text = data.sSkillName;
        text_SkilDescription.text = data.sSkillDescription;
    }




    // 마우스 올림
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!bCanInteract) return;

        // 기존 트윈 중지
        transform.DOKill();
        image_BackGround.DOKill();

        // 부드러운 크기 축소
        transform.DOScale(originalScale * scaleFactor, tweenDuration);

        // 배경 채도 낮추기(회색톤과 블렌드)
        Color targetColor = Color.Lerp(originalBGColor, Color.gray, 0.5f);
        image_BackGround.DOColor(targetColor, tweenDuration);

        // Debug.Log($"[SkillCard_UI] Enter '{text_SkillName.text}'");
    }


    // 마우스 커서 내림
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!bCanInteract) return;

        // 기존 트윈 중지
        transform.DOKill();
        image_BackGround.DOKill();

        // 원래 크기, 컬러로 복귀
        transform.DOScale(originalScale, tweenDuration);
        image_BackGround.DOColor(originalBGColor, tweenDuration);

        // Debug.Log($"[SkillCard_UI] Exit '{text_SkillName.text}'");
    }





    // 마우스 커서 클릭
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!bCanInteract) return;
        skillCardController.SetAllCanInteract(false);



        // Debug.Log($"[SkillCard_UI] Clicked '{text_SkillName.text}'");

        // 클릭하면 Exit 효과 실행
        OnPointerExit(eventData);

        // 클릭 애니메이션: 펀치 후 HideAll 호출
        transform.DOKill();
        Sequence clickSeq = DOTween.Sequence();
        clickSeq.Append(transform.DOPunchScale(originalScale * 0.1f, tweenDuration + 0.5f));
        clickSeq.Append(transform.DOScale(originalScale, tweenDuration));
        clickSeq.OnComplete(() =>
        {
            // 애니메이션이 끝난 뒤에야 선택 상태 해제
            skillCardController.HideAll();
        });
    }
}
