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


    [Header("UI Card에 표시할 내용들")]
    public Image image_Cardillustration;
    public Image image_CardBorderLine;
    public Image image_CardTitle;
    public Image image_CardKeyWord;

    public TMP_Text text_Description;

    [Header("Skill 스크립터블")]
    public SkillCard_SO skillCard_SO;


    // 트위닝 관리용

    private RectTransform rectTransformMine;
    private Vector3 originalScale;

    private Vector2 originalTitleAnchorPos;
    private Color originalIllustrationColor;
    private Color originalKeyWordColor;

    private Color originalDescriptionColor;

    private float tweenDuration = 0.08f;
    private float scaleFactor = 1.1f;


    private void Awake()
    {
        rectTransformMine = GetComponent<RectTransform>();
        originalScale = transform.localScale;

        // 복원용 정보 저장
        originalTitleAnchorPos = image_CardTitle.rectTransform.anchoredPosition;
        originalIllustrationColor = image_Cardillustration.color;
        originalKeyWordColor = image_CardKeyWord.color;

        Color startColor = originalDescriptionColor;
        startColor.a = 0f;
        text_Description.color = startColor;


    }


    // Card에 표현할 내용 적용
    public void ApplyData(SkillCard_SO data)
    {
        if (data == null) return;

        skillCard_SO = data;

        image_Cardillustration.sprite = data.sprite_Cardillustration;
        image_CardBorderLine.sprite = data.sprite_CardBorderLine;
        image_CardTitle.sprite = data.sprite_CardTitle;
        image_CardKeyWord.sprite = data.sprite_CardKeyWord;

        text_Description.text = data.description;
    }



    // 마우스 올렸을 때 나오는 애니메이션 
    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (!bCanInteract || skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber) return;
        if (!bCanInteract) return;

        transform.DOScale(originalScale * scaleFactor, tweenDuration);

        Color targetGray = new Color(50f / 255f, 50f / 255f, 50f / 255f, image_Cardillustration.color.a);
        image_Cardillustration.DOColor(targetGray, tweenDuration);

        Vector2 targetPos = originalTitleAnchorPos + new Vector2(0, -100f);
        image_CardTitle.rectTransform.DOAnchorPos(targetPos, tweenDuration);

        Color transparent = originalKeyWordColor;
        transparent.a = 0f;
        image_CardKeyWord.DOColor(transparent, tweenDuration);

        Color fullVisible = originalDescriptionColor;
        fullVisible.a = 1f;
        text_Description.DOColor(fullVisible, tweenDuration);
    }



    // 마우스 내렸을 때 나오는 애니메이션 
    public void OnPointerExit(PointerEventData eventData)
    {
        // if (!bCanInteract || skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber) return;
        if (!bCanInteract) return;

        transform.DOScale(originalScale, tweenDuration);

        image_Cardillustration.DOColor(originalIllustrationColor, tweenDuration);
        image_CardTitle.rectTransform.DOAnchorPos(originalTitleAnchorPos, tweenDuration);
        image_CardKeyWord.DOColor(originalKeyWordColor, tweenDuration);

        Color invisible = originalDescriptionColor;
        invisible.a = 0f;
        text_Description.DOColor(invisible, tweenDuration);
    }
    public void ResetCardAnim()
    {
        transform.DOScale(originalScale, tweenDuration);

        image_Cardillustration.DOColor(originalIllustrationColor, tweenDuration);
        image_CardTitle.rectTransform.DOAnchorPos(originalTitleAnchorPos, tweenDuration);
        image_CardKeyWord.DOColor(originalKeyWordColor, tweenDuration);

        // description → 알파값 0
        Color invisible = originalDescriptionColor;
        invisible.a = 0f;
        text_Description.DOColor(invisible, tweenDuration);
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        // if (!bCanInteract || skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber) return;
        if (!bCanInteract) return;

        //GameObject skillObj = skillCardController.CreateSkillInstance(skillCard_SO);
        //if (skillObj == null)
        //{
        //    Debug.LogError("[SkillCard_UI] Failed to create skill prefab.");
        //    return;
        //}

        //SkillWorker targetWorker = (MatchResultStore.myPlayerNumber == 1)
        //    ? skillCardController.InGameUiController.gameManager.skillWorker_1
        //    : skillCardController.InGameUiController.gameManager.skillWorker_2;

        //targetWorker.EquipSkillByCard(skillObj);

        //Debug.Log("I selected " + skillCard_SO.sSkillName);

        //P2PMessageSender.SendMessage(
        //    SkillSelectBuilder.Build(MatchResultStore.myPlayerNumber, skillCard_SO.sSkillName)
        //);

        // UI 처리
        skillCardController.SetAllCanInteract(false);
        skillCardController.iAuthorityPlayerNum = 0;
        transform.DOKill();

        skillCardController.HideSkillCardList(skillCard_SO.iAnimalNum, rectTransformMine.anchoredPosition);

        //OnPointerExit(eventData);

        //Sequence clickSeq = DOTween.Sequence();
        //clickSeq.Append(transform.DOPunchScale(originalScale * 0.1f, tweenDuration + 0.5f));
        //clickSeq.Append(transform.DOScale(originalScale, tweenDuration));
        //clickSeq.OnComplete(() =>
        //{
        //    skillCardController.HideSkillCardList();
        //});
    }





    public void StartFloatingAnimation()
    {
        float randomOffset = Random.Range(0f, 2f);
        float randomSpeed = Random.Range(0.8f, 1.2f);

        // 위아래 움직임
        rectTransformMine.DOAnchorPosY(rectTransformMine.anchoredPosition.y + Random.Range(-20f, 20f), randomSpeed + randomOffset)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);

        // 좌우 움직임 (다른 속도와 범위로)
        float randomSpeedX = Random.Range(1.2f, 1.8f);
        rectTransformMine.DOAnchorPosX(rectTransformMine.anchoredPosition.x + Random.Range(-20f, 20f), randomSpeedX + randomOffset)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }
    public void StopFloatingAnimation()
    {
        rectTransformMine.DOKill();
    }

}
