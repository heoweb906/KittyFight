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
    public Image image_BackGround;
    public Image image_SkillIcon;
    public TextMeshProUGUI text_SkillName;
    public TextMeshProUGUI text_SkilDescription;

    [Header("Skill 스크립터블")]
    public SkillCard_SO skillCard_SO;
     

    // 트위닝 관리용
    private Vector3 originalScale;
    private Color originalBGColor;
    private float tweenDuration = 0.08f;
    private float scaleFactor = 0.95f;


    private void Awake()
    {
        originalScale = transform.localScale;
        originalBGColor = image_BackGround.color;
    }


    // Card에 표현할 내용 적용
    public void ApplyData(SkillCard_SO data)
    {
        if (data == null) return;

        skillCard_SO = data;
        if (data.image_BackGround != null && data.image_BackGround.sprite != null)
            image_BackGround.sprite = data.image_BackGround.sprite;

        if (data.image_SkillIcon != null && data.image_SkillIcon.sprite != null)
            image_SkillIcon.sprite = data.image_SkillIcon.sprite;

        text_SkillName.text = data.sSkillName;
        text_SkilDescription.text = data.sSkillDescription;
    }


    // 마우스 올렸을 때 나오는 애니메이션 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!bCanInteract || skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber) return;

        transform.DOKill();
        image_BackGround.DOKill();

        transform.DOScale(originalScale * scaleFactor, tweenDuration);

        Color targetColor = Color.Lerp(originalBGColor, Color.gray, 0.5f);
        image_BackGround.DOColor(targetColor, tweenDuration);
    }

    // 마우스 내렸을 때 나오는 애니메이션 
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!bCanInteract || skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber) return;

        transform.DOKill();
        image_BackGround.DOKill();

        transform.DOScale(originalScale, tweenDuration);
        image_BackGround.DOColor(originalBGColor, tweenDuration);
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (!bCanInteract || skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber) return;

        // 프리팹 생성
        GameObject skillObj = skillCardController.CreateSkillInstance(skillCard_SO);
        if (skillObj == null)
        {
            Debug.LogError("[SkillCard_UI] Failed to create skill prefab.");
            return;
        }

        // 내 번호에 따라 대상 SkillWorker 선택
        SkillWorker targetWorker = (MatchResultStore.myPlayerNumber == 1)
            ? skillCardController.InGameUiController.gameManager.skillWorker_1
            : skillCardController.InGameUiController.gameManager.skillWorker_2;

        // 장착 시도 (슬롯은 SkillWorker 내부에서 자동 결정)
        targetWorker.EquipSkillByCard(skillObj);

        Debug.Log("I selected " + skillCard_SO.sSkillName);

        // 메시지 전송
        P2PMessageSender.SendMessage(
            SkillSelectBuilder.Build(MatchResultStore.myPlayerNumber, skillCard_SO.sSkillName)
        );

        // UI 처리
        skillCardController.SetAllCanInteract(false);
        skillCardController.iAuthorityPlayerNum = 0;

        OnPointerExit(eventData);
        transform.DOKill();

        Sequence clickSeq = DOTween.Sequence();
        clickSeq.Append(transform.DOPunchScale(originalScale * 0.1f, tweenDuration + 0.5f));
        clickSeq.Append(transform.DOScale(originalScale, tweenDuration));
        clickSeq.OnComplete(() =>
        {
            skillCardController.HideSkillCardList();
        });
    }
}
