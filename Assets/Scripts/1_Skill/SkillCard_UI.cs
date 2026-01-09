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

    public bool bIsRat = false;


    [Header("UI Card에 표시할 내용들")]
    public CanvasGroup imageGroup_illustration;
    public Image image_GrayWall;
    public Image image_DrawTeritory;
    public Image image_CardBorderLine;
    public Image image_CardAnilSimbol;
    public Image image_CardTitle;
    public Image image_CardKeyWord;
    public Image image_BorderLine_Left;
    public Image image_BorderLine_Right;
    public Image image_Triangle;
    public TMP_Text text_Description;

    [Header("Skill 스크립터블")]
    public SkillCard_SO skillCard_SO;
    private CardAnimationBase currentAnimation;


    // 트위닝 관리용

    private RectTransform rectTransformMine;
    private Vector3 originalScale;

    private Vector2 originalTitleAnchorPos;
    private Color originalKeyWordColor;

    private Color originalDescriptionColor;

    private Vector2 originalBorderLeftPos;
    private Vector2 originalBorderRightPos;

    private float tweenDuration = 0.08f;
    private float scaleFactor = 1.15f;


    private void Awake()
    {
        rectTransformMine = GetComponent<RectTransform>();
        originalScale = transform.localScale;

        // 복원용 정보 저장
        originalTitleAnchorPos = image_CardTitle.rectTransform.anchoredPosition;
        originalKeyWordColor = image_CardKeyWord.color;
        originalBorderLeftPos = image_BorderLine_Left.rectTransform.anchoredPosition;
        originalBorderRightPos = image_BorderLine_Right.rectTransform.anchoredPosition;

        Color startColor = originalDescriptionColor;
        startColor.a = 0f;
        text_Description.color = startColor;

        // =========================================================
        // [추가] 모든 카드 공통 적용: 마스킹 처리
        // =========================================================

        // 1. GrayWall에 Mask 컴포넌트 추가 (이미지 모양대로 잘라냄)
        Mask wallMask = image_DrawTeritory.GetComponent<Mask>();
        if (wallMask == null)
        {
            // [수정] image_CardBorderLine -> image_DrawTeritory로 변경
            wallMask = image_DrawTeritory.gameObject.AddComponent<Mask>();
            wallMask.showMaskGraphic = true;
        }

        // 2. 일러스트 그룹을 DrawTeritory 자식으로 이동 (이제 마스크 영향 받음)
        imageGroup_illustration.transform.SetParent(image_DrawTeritory.transform, false);
    }


    // Card에 표현할 내용 적용
    public void ApplyData(SkillCard_SO data, bool bIsRat = false)
    {
        if (data == null) return;

        skillCard_SO = data;

        // 기존 애니메이션 컴포넌트 제거
        if (currentAnimation != null)
        {
            currentAnimation.StopAnimation();
            Destroy(currentAnimation);
            currentAnimation = null;
        }

        // 기존 일러스트 이미지들 삭제
        for (int i = imageGroup_illustration.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(imageGroup_illustration.transform.GetChild(i).gameObject);
        }

        // 새로운 일러스트 이미지들 생성
        if (data.cardillustrationPivots != null && data.cardillustrationPivots.Length > 0)
        {
            for (int i = 0; i < data.cardillustrationPivots.Length; i++)
            {
                GameObject imageObj = new GameObject("Illustration_" + i);
                imageObj.transform.SetParent(imageGroup_illustration.transform, false);
                Image img = imageObj.AddComponent<Image>();
                img.sprite = data.cardillustrationPivots[i].sprite_Cardillustration;
                img.raycastTarget = false;
                RectTransform rect = img.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
                var margin = data.cardillustrationPivots[i].margins;
                rect.offsetMin = new Vector2(margin.left, margin.bottom);
                rect.offsetMax = new Vector2(-margin.right, -margin.top);
            }
        }

        // 애니메이션 컴포넌트 추가
        AddAnimationComponent(data.AnimationType);

        image_CardBorderLine.sprite = data.sprite_Frame;
        image_CardAnilSimbol.sprite = data.sprite_Simbol;
        image_CardTitle.sprite = data.sprite_SkillName;
        image_CardKeyWord.sprite = data.sprite_Keyword;
        image_BorderLine_Left.sprite = data.sprite_BorderLine_Left;
        image_BorderLine_Right.sprite = data.sprite_BorderLine_Right;

        text_Description.text = data.description;
    }


    private void AddAnimationComponent(CardAnimationType animationType)
    {
        // enum 이름에서 숫자 부분 추출 (예: "Number_5" -> "5")
        string enumName = animationType.ToString();
        string number = enumName.Replace("Number_", "");

        // 클래스 이름 생성 (예: "CardAnimation_Num_5")
        string className = $"CardAnimation_Num_{number}";

        // Type 가져오기
        System.Type animationClassType = System.Type.GetType(className);

        if (animationClassType != null && animationClassType.IsSubclassOf(typeof(CardAnimationBase)))
        {
            currentAnimation = gameObject.AddComponent(animationClassType) as CardAnimationBase;
        }
        else
        {
            Debug.LogError($"애니메이션 클래스를 찾을 수 없습니다: {className}");
        }
    }


    public void StartCardAnimation()
    {
        if (currentAnimation != null)
        {
            List<Image> images = new List<Image>();
            for (int i = 0; i < imageGroup_illustration.transform.childCount; i++)
            {
                Image img = imageGroup_illustration.transform.GetChild(i).GetComponent<Image>();
                if (img != null) images.Add(img);
            }
            currentAnimation.StartAnimation(images);
        }

        // 보더라인 애니메이션 시작
        StartBorderLineAnimation();
    }


    public void StopCardAnimation()
    {
        if (currentAnimation != null)
        {
            currentAnimation.StopAnimation();
        }

        // 보더라인 애니메이션 정지
        StopBorderLineAnimation();
    }


    // 마우스 올렸을 때 나오는 애니메이션 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!bCanInteract /*|| skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber */) return;

        transform.DOScale(originalScale * scaleFactor, tweenDuration);

        // 회색 벽 알파값을 244로 (기존 이미지 색상 변경 코드 제거)
        if(image_GrayWall != null) image_GrayWall.DOFade(248f / 255f, tweenDuration);

        skillCardController.InGameUiController.PlaySFX(skillCardController.InGameUiController.sfxClips_InGameSystem[8]);



        Vector2 targetPos = originalTitleAnchorPos + new Vector2(0, -50f);
        image_CardTitle.rectTransform.DOAnchorPos(targetPos, tweenDuration);
        Color transparent = originalKeyWordColor;
        transparent.a = 0f;
        image_CardKeyWord.DOColor(transparent, tweenDuration);
        Color fullVisible = originalDescriptionColor;
        fullVisible.a = 1f;
        text_Description.DOColor(fullVisible, tweenDuration);

        // 보더라인 알파값 0으로
        image_Triangle.DOFade(0f, tweenDuration);
        image_BorderLine_Left.DOFade(0f, tweenDuration);
        image_BorderLine_Right.DOFade(0f, tweenDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!bCanInteract /*|| skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber */) return;

        transform.DOScale(originalScale, tweenDuration);

        // 회색 벽 알파값을 0으로 (기존 이미지 색상 복원 코드 제거)
        if (image_GrayWall != null)  image_GrayWall.DOFade(0f, tweenDuration);

        image_CardTitle.rectTransform.DOAnchorPos(originalTitleAnchorPos, tweenDuration);
        image_CardKeyWord.DOColor(originalKeyWordColor, tweenDuration);
        Color invisible = originalDescriptionColor;
        invisible.a = 0f;
        text_Description.DOColor(invisible, tweenDuration);

        // 보더라인 알파값 1로 복원
        image_Triangle.DOFade(1f, tweenDuration);
        image_BorderLine_Left.DOFade(1f, tweenDuration);
        image_BorderLine_Right.DOFade(1f, tweenDuration);
    }


    public void ResetCardAnim()
    {
        transform.DOScale(originalScale, tweenDuration);

        if (image_GrayWall != null)  image_GrayWall.DOFade(0f, tweenDuration);

        imageGroup_illustration.DOFade(1f, tweenDuration);
        image_CardTitle.rectTransform.DOAnchorPos(originalTitleAnchorPos, tweenDuration);
        image_CardKeyWord.DOColor(originalKeyWordColor, tweenDuration);

        // description → 알파값 0
        Color invisible = originalDescriptionColor;
        invisible.a = 0f;
        text_Description.DOColor(invisible, tweenDuration);
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. 상호작용 가능 여부 및 권한 확인
        if (!bCanInteract || skillCardController.iAuthorityPlayerNum != MatchResultStore.myPlayerNumber) return;

        // 2. [수정] 내 플레이어의 Ability를 먼저 가져옵니다 (보유 스킬 체크용)
        PlayerAbility targetPlayerAbility = (MatchResultStore.myPlayerNumber == 1)
            ? skillCardController.InGameUiController.gameManager.playerAbility_1
            : skillCardController.InGameUiController.gameManager.playerAbility_2;

        SkillCard_SO skillToEquip = skillCard_SO;
        int randomSkillID = -1;
        int targetListIndex = -1; // 리스트의 실제 인덱스 저장 변수

        // 3. 쥐(Rat) 카드일 경우 로직 수행
        if (bIsRat)
        {
            // 액티브/패시브 여부에 따라 다른 배열 사용
            int[] ratSkillPool;

            if (skillCard_SO.iSkillIndex < 100) // 현재 보여진 카드가 액티브 구간이면
            {
                ratSkillPool = new int[] { 1, 2 }; // 액티브 스킬 풀
            }
            else // 패시브 구간이면
            {
                ratSkillPool = new int[] { 101, 102, 139 }; // 패시브 스킬 풀
            }

            // =================================================================
            // [핵심 수정] "내 캐릭터가 이미 가진 스킬"은 후보에서 제외
            // =================================================================
            List<int> availableCandidates = new List<int>();

            foreach (int id in ratSkillPool)
            {
                bool isOwned = false;

                // A. 액티브 스킬 체크 (1, 2번 등)
                if (id < 100)
                {
                    if (targetPlayerAbility.skill1 != null && targetPlayerAbility.skill1.SkillIndex == id) isOwned = true;
                    if (targetPlayerAbility.skill2 != null && targetPlayerAbility.skill2.SkillIndex == id) isOwned = true;
                }
                // B. 패시브 스킬 체크 (101번 등)
                else
                {
                    // passives 리스트를 순회하며 ID 확인
                    if (targetPlayerAbility.passives != null)
                    {
                        foreach (var p in targetPlayerAbility.passives)
                        {
                            if (p != null && p.PassiveId == id)
                            {
                                isOwned = true;
                                break;
                            }
                        }
                    }
                }

                // 내가 안 가지고 있으면 후보에 추가
                if (!isOwned)
                {
                    availableCandidates.Add(id);
                }
            }

            // 후보가 하나라도 있으면 거기서 뽑고, 없으면(다 가졌으면) 전체 풀에서 랜덤(중복 허용)
            if (availableCandidates.Count > 0)
            {
                randomSkillID = availableCandidates[Random.Range(0, availableCandidates.Count)];
            }
            else
            {
                randomSkillID = ratSkillPool[Random.Range(0, ratSkillPool.Length)];
            }
            // =================================================================

            // 추출한 ID에 해당하는 스킬 데이터와 '리스트 인덱스' 찾기
            for (int i = 0; i < skillCardController.skillDataList.Count; i++)
            {
                if (skillCardController.skillDataList[i].iSkillIndex == randomSkillID)
                {
                    skillToEquip = skillCardController.skillDataList[i];
                    targetListIndex = i;
                    break;
                }
            }
        }

        // 4. 스킬 오브젝트 생성 및 장착
        GameObject skillObj = skillCardController.CreateSkillInstance(skillToEquip);

        if (skillToEquip.cardType == CardType.Active)
        {
            Skill skillComponent = skillObj.GetComponent<Skill>();
            if (skillComponent != null)
            {
                SkillType targetSlot = targetPlayerAbility.GetSkill(SkillType.Skill1) == null ? SkillType.Skill1 : SkillType.Skill2;
                targetPlayerAbility.SetSkill(targetSlot, skillComponent);
            }
        }
        else if (skillToEquip.cardType == CardType.Passive)
        {
            Passive passiveComponent = skillObj.GetComponent<Passive>();
            if (passiveComponent != null)
            {
                targetPlayerAbility.EquipPassive(passiveComponent);
            }
        }

        int realEquippedSkillID = bIsRat ? randomSkillID : skillCard_SO.iSkillIndex;
        skillCardController.MarkSkillAsUsed(realEquippedSkillID);

        // P2P 메시지 전송
        P2PMessageSender.SendMessage(
              SkillSelectBuilder.Build(
                  MatchResultStore.myPlayerNumber,
                  skillToEquip.sSkillName,
                  rectTransformMine.anchoredPosition,
                  skillCard_SO, // 상대방에게는 '원래 카드(호랑이)'로 보이게 전송
                  bIsRat,
                  randomSkillID
              )
          );

        // UI 처리
        skillCardController.SetBoolAllCardInteract(false);
        skillCardController.iAuthorityPlayerNum = 0;
        transform.DOKill();

        if (bIsRat)
        {
            skillCardController.HIdeSkillCardList_ForRat(skillCard_SO.iSkillIndex, transform.position, randomSkillID);
        }
        else
        {
            skillCardController.HideSkillCardList(skillCard_SO.iSkillIndex, rectTransformMine.anchoredPosition);
        }
    }


    private void StartBorderLineAnimation()
    {
        Vector2 leftOriginalPos = image_BorderLine_Left.rectTransform.anchoredPosition;
        Vector2 leftTargetPos = leftOriginalPos + new Vector2(-20f, 20f); // 좌측 상단
        image_BorderLine_Left.rectTransform.DOAnchorPos(leftTargetPos, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);


        Vector2 rightOriginalPos = image_BorderLine_Right.rectTransform.anchoredPosition;
        Vector2 rightTargetPos = rightOriginalPos + new Vector2(20f, 20f); // 우측 상단
        image_BorderLine_Right.rectTransform.DOAnchorPos(rightTargetPos, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopBorderLineAnimation()
    {
        image_BorderLine_Left.rectTransform.DOKill();
        image_BorderLine_Right.rectTransform.DOKill();

        // 원래 위치로 복원
        image_BorderLine_Left.rectTransform.anchoredPosition = originalBorderLeftPos;
        image_BorderLine_Right.rectTransform.anchoredPosition = originalBorderRightPos;
    }



    public void StartFloatingAnimation()
    {
        float randomOffset = Random.Range(0f, 1.5f);
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