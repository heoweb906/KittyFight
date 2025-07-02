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
        if (!bCanInteract) return;

        transform.DOKill();
        image_BackGround.DOKill();

        transform.DOScale(originalScale * scaleFactor, tweenDuration);

        Color targetColor = Color.Lerp(originalBGColor, Color.gray, 0.5f);
        image_BackGround.DOColor(targetColor, tweenDuration);
    }

    // 마우스 내렸을 때 나오는 애니메이션 
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!bCanInteract) return;

        transform.DOKill();
        image_BackGround.DOKill();

        transform.DOScale(originalScale, tweenDuration);
        image_BackGround.DOColor(originalBGColor, tweenDuration);
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (!bCanInteract) return;

        var matchManager = FindObjectOfType<MatchManager>();
        if (matchManager == null)
        {
            Debug.LogError("[SkillCard_UI] MatchManager를 찾을 수 없습니다.");
            return;
        }

        // 내 플레이어 객체 가져오기 (실제 로직에 맞게 수정)
        //GameObject myPlayer = matchManager.player1Object;
        //var mySkillWorker = myPlayer.GetComponent<SkillWorker>();
        //if (mySkillWorker == null)
        //{
        //    Debug.LogError("[SkillCard_UI] 내 캐릭터에서 SkillWorker를 찾을 수 없습니다.");
        //    return;
        //}

        //SkillSlotType selectedSlot = SkillSlotType.Q;

        //// 수정된 부분: sSkillName 대신 SkillCard_SO 인스턴스를 전달
        //mySkillWorker.EquipSkillByCard(selectedSlot, skillCard_SO);

        // 네트워크 전송, 인터랙션 비활성화 등 나머지 로직은 동일
        //string slot = selectedSlot.ToString();
        //string msg = SkillBuilder.Build(skillCard_SO.sSkillName, /*playerNumber*/ 1, slot);
        //P2PSkillSender.SendMessage(msg);

        //skillCardController.SetAllCanInteract(false);
        //OnPointerExit(eventData);

        //transform.DOKill();
        //Sequence clickSeq = DOTween.Sequence();
        //clickSeq.Append(transform.DOPunchScale(originalScale * 0.1f, tweenDuration + 0.5f));
        //clickSeq.Append(transform.DOScale(originalScale, tweenDuration));
        //clickSeq.OnComplete(() =>
        //{
        //    skillCardController.HideAll();
        //});
    }
}
