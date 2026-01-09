using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class SkillCardController : MonoBehaviour
{
    [Header("중요한 정보들")]
    public int iAuthorityPlayerNum = 0;
    public InGameUIController InGameUiController { get; set; }
    public bool IsAnimating { get; private set; }

    public TMP_Text text_Timer;
    private float fTimerInternal;
    public int iTimerForSelect;
    public bool bTimerCheck;

    [Header("리소스 설정")]
    [Tooltip("Resources/SkillCards 폴더 경로")]
    [SerializeField] string skillCardResourceFolder = "SkillCards";

    [Header("스킬 프리팹 리스트")]
    public List<GameObject> skillPrefabs;

    [Header("스킬 카드 프리팹")]
    [SerializeField] GameObject objSkillCard;
    [SerializeField] GameObject objSkillCard_Rat;
    public List<SkillCard_SO> skillDataList = new List<SkillCard_SO>();
    private SkillCard_UI[] instances = new SkillCard_UI[4];

    [Header("카드 생성 위치 배열")]
    [SerializeField] Transform[] spawnPoints = new Transform[4];
    [SerializeField] Transform[] targetPoints = new Transform[4];

    [Header("연출에 사용할 것들")]
    public GameObject objs_AnimalSkillCardEffects;
    public Sprite[] sprites_Icons;
    public Sprite sprite_RatHeart;
    public Transform ransform_RatCardLeft;
    public RectTransform rasnform_RatIconLeft;
    public Transform ransform_RatCardRight;
    public RectTransform rasnform_RatIconRight;
    private Dictionary<int, Sprite> skillIconMap = new Dictionary<int, Sprite>();

    private Sequence ratSequence;
    private GameObject currentTargetIcon;
    private GameObject currentHeartObj;
    private GameObject currentRewardIcon;

    public HashSet<int> usedSkillIDs = new HashSet<int>();

    // [추가] 안전한 종료를 위해 OnDestroy에서 정리
    private void OnDestroy()
    {
        // 실행 중인 모든 쥐 연출 시퀀스 즉시 종료
        if (ratSequence != null) ratSequence.Kill();

        // DOTween이 이 스크립트의 GameObject에 연결된 것들을 정리
        transform.DOKill();
    }

    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;
        var datas = Resources.LoadAll<SkillCard_SO>(skillCardResourceFolder);

        int ExtractLeadingNumber(string name)
        {
            var match = Regex.Match(name, @"^\d+");
            if (match.Success)
                return int.Parse(match.Value);
            return int.MaxValue;
        }

        var sortedDatas = datas.OrderBy(d => ExtractLeadingNumber(d.name)).ToArray();

        foreach (var data in sortedDatas)
        {
            if (ExtractLeadingNumber(data.name) != 0)
            {
                skillDataList.Add(data);
            }
        }

        for (int i = 0; i < instances.Length; i++)
        {
            if (objSkillCard == null || spawnPoints[i] == null)
            {
                Debug.LogError($"[SkillCardController] Prefab 또는 spawnPoints[{i}]가 할당되지 않았습니다.");
                continue;
            }

            var go = Instantiate(objSkillCard, spawnPoints[i].position, Quaternion.identity, parent);
            var card = go.GetComponent<SkillCard_UI>();
            card.skillCardController = this;

            if (card == null)
            {
                Debug.LogError("[SkillCardController] objSkillCard Prefab에 SkillCard_UI 컴포넌트가 없습니다.");
                continue;
            }

            card.ApplyData(null, false);
            card.gameObject.SetActive(false);
            instances[i] = card;
        }

        for (int i = 0; i < instances.Length; i++)
        {
            instances[i].bIsRat = false;
        }

        if (Random.Range(0f, 100f) < 10f)
        {
            int randomIndex = Random.Range(0, instances.Length);
            instances[randomIndex].bIsRat = true;
        }

        MapSkillIcons();

        text_Timer.gameObject.SetActive(false);
        iTimerForSelect = 0;
        fTimerInternal = 0f;
        bTimerCheck = false;
    }

    private void Update()
    {
        if (iTimerForSelect > 0 && bTimerCheck)
        {
            fTimerInternal -= Time.deltaTime;

            int newTimerValue = Mathf.CeilToInt(fTimerInternal);
            if (newTimerValue != iTimerForSelect)
            {
                iTimerForSelect = newTimerValue;
                text_Timer.text = iTimerForSelect.ToString();
            }

            if (fTimerInternal <= 0 && iAuthorityPlayerNum == MatchResultStore.myPlayerNumber)
            {
                iTimerForSelect = 0;
                fTimerInternal = 0;
                bTimerCheck = false;

                SelectRandomCard();
            }
        }
    }

    public void ShowSkillCardList(int iPlayernum = 0, bool bActivePassive = true, int[] iCardArray = null)
    {
        if (skillDataList.Count == 0 || IsAnimating) return;

        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[6]);
        iAuthorityPlayerNum = iPlayernum;
        IsAnimating = true;

        PlayerAbility currentPlayerAbility = null;

        if (InGameUiController?.gameManager != null)
        {
            if (iPlayernum == 1) currentPlayerAbility = InGameUiController.gameManager.playerAbility_1;
            else if (iPlayernum == 2) currentPlayerAbility = InGameUiController.gameManager.playerAbility_2;
        }

        List<int> selectedIndices = new List<int>();

        if (iCardArray != null && iCardArray.Length > 0)
        {
            for (int i = 0; i < instances.Length && i < iCardArray.Length; i++)
            {
                int cardIndex = Mathf.Clamp(iCardArray[i], 0, skillDataList.Count - 1);
                selectedIndices.Add(cardIndex);
            }
            StartShowingCards(selectedIndices);
        }
        else
        {
            List<int> filteredIndices = new List<int>();
            int[] excludedSkillIndices = { 1, 2, 101, 102, 139 };

            for (int i = 0; i < skillDataList.Count; i++)
            {
                int currentSkillID = skillDataList[i].iSkillIndex;
                if (excludedSkillIndices.Contains(currentSkillID)) continue;
                if (usedSkillIDs.Contains(currentSkillID)) continue;

                bool isActive = currentSkillID < 100;
                if ((bActivePassive && isActive) || (!bActivePassive && !isActive))
                {
                    filteredIndices.Add(i);
                }
            }

            List<int> availableIndices = new List<int>(filteredIndices);

            for (int i = availableIndices.Count - 1; i >= 0; i--)
            {
                int currentID = skillDataList[availableIndices[i]].iSkillIndex;
                if (IsSkillOwned(currentPlayerAbility, currentID))
                {
                    availableIndices.RemoveAt(i);
                }
            }

            if (availableIndices.Count < instances.Length)
            {
                foreach (int idx in filteredIndices)
                {
                    if (!availableIndices.Contains(idx)) availableIndices.Add(idx);
                    if (availableIndices.Count >= instances.Length) break;
                }
            }

            for (int i = 0; i < instances.Length && availableIndices.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
                int selectedIdx = availableIndices[randomIndex];
                selectedIndices.Add(selectedIdx);
                availableIndices.RemoveAt(randomIndex);
            }

            if (selectedIndices.Count < instances.Length)
            {
                List<int> safeFallbackIndices = new List<int>();
                for (int i = 0; i < skillDataList.Count; i++)
                {
                    int id = skillDataList[i].iSkillIndex;
                    if (excludedSkillIndices.Contains(id)) continue;
                    bool isActive = id < 100;
                    if ((bActivePassive && isActive) || (!bActivePassive && !isActive))
                    {
                        safeFallbackIndices.Add(i);
                    }
                }

                if (safeFallbackIndices.Count > 0)
                {
                    while (selectedIndices.Count < instances.Length)
                    {
                        int randomSafeIdx = safeFallbackIndices[UnityEngine.Random.Range(0, safeFallbackIndices.Count)];
                        selectedIndices.Add(randomSafeIdx);
                    }
                }
            }

            P2PMessageSender.SendMessage(
                SkillShowBuilder.Build(MatchResultStore.myPlayerNumber, selectedIndices.ToArray())
            );

            StartShowingCards(selectedIndices);
        }
    }

    private void StartShowingCards(List<int> selectedIndices)
    {
        int completed = 0;
        int total = instances.Length;

        FadeImage(1f, 0.2f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.1f, () => FadeImage(0f, 0.5f)).SetLink(gameObject);

            if (InGameUiController?.scoreBoardUIController != null)
            {
                InGameUiController.scoreBoardUIController.ActiveFalseBones();
                InGameUiController.scoreBoardUIController.OnOffScoreTextObj(false);
            }

            if (text_Timer != null)
            {
                text_Timer.gameObject.SetActive(true);
                iTimerForSelect = 15;
                fTimerInternal = 15.0f;
                bTimerCheck = true;
                text_Timer.text = iTimerForSelect.ToString();
            }

            for (int i = 0; i < total; i++)
            {
                try
                {
                    var card = instances[i];

                    if (card == null || targetPoints[i] == null)
                    {
                        completed++;
                        if (completed >= total) CompleteCardShow();
                        continue;
                    }

                    if (i < selectedIndices.Count)
                    {
                        int idx = selectedIndices[i];
                        card.ApplyData(skillDataList[idx], false);
                        card.ResetCardAnim();
                        card.gameObject.SetActive(true);
                        card.StartCardAnimation();

                        // [수정] 이전에 실행 중이던 이동 트윈 제거
                        card.transform.DOKill();

                        var currentCard = card;
                        card.transform.DOMove(targetPoints[i].position, 0.3f)
                            .SetLink(currentCard.gameObject) // [핵심] 카드 파괴 시 트윈 삭제
                            .OnComplete(() =>
                            {
                                if (currentCard != null) // 안전 체크
                                {
                                    currentCard.StartFloatingAnimation();
                                }
                                completed++;
                                if (completed >= total) CompleteCardShow();
                            })
                            .OnKill(() =>
                            {
                                // 트윈이 강제 종료되었을 때의 처리 (필요시)
                            });
                    }
                    else
                    {
                        completed++;
                        if (completed >= total) CompleteCardShow();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SkillCardController] {i}번 카드 에러: {e.Message}");
                    completed++;
                    if (completed >= total) CompleteCardShow();
                }
            }
        }).SetLink(gameObject); // FadeImage의 OnComplete도 안전하게
    }

    private void CompleteCardShow()
    {
        IsAnimating = false;
        SetBoolAllCardInteract(true);
    }

    private bool IsSkillOwned(PlayerAbility playerAbility, int skillIndex)
    {
        if (playerAbility == null) return false;
        if (playerAbility.skill1 != null && playerAbility.skill1.SkillIndex == skillIndex) return true;
        if (playerAbility.skill2 != null && playerAbility.skill2.SkillIndex == skillIndex) return true;
        return false;
    }

    public void HideSkillCardList(int iSkillIndex = 0, Vector2 clickedCardPosition = default)
    {
        if (IsAnimating) return;
        IsAnimating = true;
        SetBoolAllCardInteract(false);

        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[9]);

        for (int i = 0; i < instances.Length; i++)
        {
            var card = instances[i];
            if (card != null)
            {
                card.StopFloatingAnimation();
                card.transform.DOKill(); // 이동 중일 수도 있으니 킬
            }
        }

        FadeImage(1f, 0f).OnComplete(() =>
        {
            if (text_Timer != null) text_Timer.gameObject.SetActive(false);
            iTimerForSelect = 0;
            fTimerInternal = 0f;
            bTimerCheck = false;

            DOVirtual.DelayedCall(0.1f, () => FadeImage(0f, 1f)).SetLink(gameObject);
        }).SetLink(gameObject);

        for (int i = 0; i < instances.Length; i++)
        {
            var card = instances[i];
            if (card == null || spawnPoints[i] == null) continue;

            // [수정] 트윈 충돌 방지
            card.transform.DOKill();
            card.transform.position = spawnPoints[i].position;
            card.gameObject.SetActive(false);
        }

        int iTemp = iAuthorityPlayerNum;
        iAuthorityPlayerNum = 0;

        SkillCard_SO selectedSkillData = skillDataList.Find(data => data.iSkillIndex == iSkillIndex);

        if (selectedSkillData != null)
        {
            Sprite skillIconSprite = GetSkillIconBySkillIndex(selectedSkillData.iSkillIndex);

            if (skillIconSprite != null)
            {
                GameObject effectObj = new GameObject("SkillIconEffect");
                RectTransform effectRect = effectObj.AddComponent<RectTransform>();
                Image effectImage = effectObj.AddComponent<Image>();
                effectImage.sprite = skillIconSprite;
                effectRect.sizeDelta = new Vector2(200f, 200f);
                effectImage.raycastTarget = false;

                effectRect.SetParent(InGameUiController.canvasMain.transform, false);
                effectRect.localScale = Vector3.one * 1.5f;
                effectRect.SetSiblingIndex(InGameUIController.Instance.image_FadeOut_White.transform.GetSiblingIndex() - 1);
                effectRect.anchoredPosition = clickedCardPosition;

                DOVirtual.DelayedCall(1f, () =>
                {
                    if (effectRect == null) return; // 파괴 체크

                    effectRect.DOAnchorPos(Vector2.zero, 0.6f)
                        .SetEase(Ease.InBack)
                        .SetLink(effectObj) // 이펙트 오브젝트 파괴 시 트윈 중단
                        .OnComplete(() =>
                        {
                            FadeImage(1f, 0f).OnComplete(() =>
                            {
                                if (InGameUiController?.scoreBoardUIController != null)
                                {
                                    InGameUiController.scoreBoardUIController.scoreImageElement_Player1.ChangePlayerImage(4, false, 1);
                                    InGameUiController.scoreBoardUIController.scoreImageElement_Player2.ChangePlayerImage(4, false, 2);

                                    Sprite icon = GetSkillIconBySkillIndex(iSkillIndex);
                                    InGameUiController.scoreBoardUIController.scoreImageElement_Player1.imageSkillIcon.sprite = icon;
                                    InGameUiController.scoreBoardUIController.scoreImageElement_Player2.imageSkillIcon.sprite = icon;
                                    InGameUiController.scoreBoardUIController.SkillIconImageOnOff(true);
                                }

                                InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[7]);

                                DOVirtual.DelayedCall(0.1f, () =>
                                {
                                    FadeImage(0f, 1f);
                                    IsAnimating = false;
                                    DOVirtual.DelayedCall(0.9f, () =>
                                    {
                                        if (InGameUiController?.gameManager != null)
                                            InGameUiController.gameManager.ResetGame();
                                    }).SetLink(gameObject);
                                }).SetLink(gameObject);
                            }).SetLink(gameObject);

                            if (effectObj != null) Destroy(effectObj);
                        });
                }).SetLink(gameObject);
            }
            else
            {
                IsAnimating = false;
                DOVirtual.DelayedCall(1f, () =>
                {
                    if (InGameUiController?.gameManager != null)
                        InGameUiController.gameManager.ResetGame();
                }).SetLink(gameObject);
            }
        }
    }

    public void HIdeSkillCardList_ForRat(int iSkillIndex, Vector3 clickedWorldPosition, int iRaySkillIndex)
    {
        if (IsAnimating) return;

        CleanupRatEffects();
        IsAnimating = true;
        SetBoolAllCardInteract(false);
        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[9]);

        Vector2 uiAnchoredPos = WorldToCanvasPoint(clickedWorldPosition);

        // [핵심] 시퀀스 생성 및 링크 설정
        ratSequence = DOTween.Sequence();
        ratSequence.SetLink(gameObject); // 이 스크립트가 붙은 오브젝트가 파괴되면 시퀀스도 자동 Kill

        ratSequence.AppendCallback(() =>
        {
            FadeImage(1f, 0f).OnComplete(() =>
            {
                if (text_Timer != null) text_Timer.gameObject.SetActive(false);
                iTimerForSelect = 0;
                fTimerInternal = 0f;
                bTimerCheck = false;
                DOVirtual.DelayedCall(0.1f, () => FadeImage(0f, 1f)).SetLink(gameObject);
            }).SetLink(gameObject);

            for (int i = 0; i < instances.Length; i++)
            {
                if (instances[i] != null) instances[i].gameObject.SetActive(false);
            }
        });

        ratSequence.AppendCallback(() =>
        {
            Sprite skillIconSprite = GetSkillIconBySkillIndex(iSkillIndex);
            currentTargetIcon = CreateEffectImage("TargetSkillIcon", skillIconSprite, uiAnchoredPos, new Vector2(200f, 200f));

            if (currentTargetIcon != null)
            {
                RectTransform targetIconRect = currentTargetIcon.GetComponent<RectTransform>();
                // [안전장치] 부모가 유효할 때만 실행
                if (InGameUiController?.image_FadeOut_White?.transform?.parent != null)
                {
                    targetIconRect.SetParent(InGameUiController.image_FadeOut_White.transform.parent);
                    InGameUiController.image_FadeOut_White.transform.SetAsLastSibling();
                }
            }
        });

        ratSequence.AppendInterval(0.5f);

        ratSequence.AppendCallback(() =>
        {
            float canvasHeight = InGameUiController.canvasMain.GetComponent<RectTransform>().rect.height;
            Vector2 startHeartPos = new Vector2(0f, canvasHeight / 2f + 300f);

            currentHeartObj = CreateEffectImage("RatHeartMissile", sprite_RatHeart, startHeartPos, new Vector2(150f, 150f));

            if (currentHeartObj != null)
            {
                RectTransform heartRect = currentHeartObj.GetComponent<RectTransform>();
                if (InGameUiController?.image_FadeOut_White?.transform?.parent != null)
                {
                    heartRect.SetParent(InGameUiController.image_FadeOut_White.transform.parent);
                    InGameUiController.image_FadeOut_White.transform.SetAsLastSibling();
                }

                heartRect.DORotate(new Vector3(0, 0, 360 * 2), 0.6f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLink(currentHeartObj);
                heartRect.DOAnchorPos(uiAnchoredPos, 0.6f).SetEase(Ease.InQuad).SetLink(currentHeartObj);
            }
        });

        ratSequence.AppendInterval(0.6f);

        ratSequence.AppendCallback(() =>
        {
            float canvasHeight = InGameUiController.canvasMain.GetComponent<RectTransform>().rect.height;
            if (currentTargetIcon != null)
            {
                RectTransform targetIconRect = currentTargetIcon.GetComponent<RectTransform>();
                targetIconRect.DORotate(new Vector3(0, 0, 720), 0.8f, RotateMode.FastBeyond360).SetLink(currentTargetIcon);
                targetIconRect.DOAnchorPosY(-canvasHeight, 0.8f).SetEase(Ease.InBack).SetLink(currentTargetIcon);
            }
        });

        ratSequence.AppendInterval(1.5f);

        ratSequence.AppendCallback(() =>
        {
            if (currentTargetIcon != null) Destroy(currentTargetIcon);
            if (currentHeartObj != null) Destroy(currentHeartObj);

            FadeImage(1f, 0.1f).OnComplete(() =>
            {
                bool isRight = (uiAnchoredPos.x > 0);
                int realID = iRaySkillIndex;

                CreateRatCardAtTransform(isRight ? ransform_RatCardRight : ransform_RatCardLeft, realID);

                Sprite rewardIcon = GetSkillIconBySkillIndex(realID);
                currentRewardIcon = CreateEffectImage("RewardSkillIcon", rewardIcon, Vector2.zero, new Vector2(200f, 200f));

                if (currentRewardIcon != null)
                {
                    RectTransform rewardRect = currentRewardIcon.GetComponent<RectTransform>();
                    rewardRect.SetParent(InGameUiController.canvasMain.transform, false);
                    rewardRect.anchoredPosition = isRight ? new Vector2(457f, 410f) : new Vector2(-457f, 410f);
                    rewardRect.localScale = Vector3.one * 1.4f;
                }

                DOVirtual.DelayedCall(0.1f, () => FadeImage(0f, 0.5f)).SetLink(gameObject);
            }).SetLink(gameObject);
        });

        ratSequence.AppendInterval(4.4f);

        ratSequence.AppendCallback(() =>
        {
            if (currentRewardIcon != null)
            {
                RectTransform iconRect = currentRewardIcon.GetComponent<RectTransform>();
                iconRect.DOAnchorPos(Vector2.zero, 0.6f)
                    .SetEase(Ease.InBack)
                    .SetLink(currentRewardIcon)
                    .OnComplete(() => FinishRatSequence(true));
            }
            else
            {
                FinishRatSequence(false);
            }
        });
    }

    private void FinishRatSequence(bool hasIcon)
    {
        FadeImage(1f, 0f).OnComplete(() =>
        {
            if (hasIcon && currentRewardIcon != null)
            {
                Sprite iconSprite = currentRewardIcon.GetComponent<Image>().sprite;
                if (InGameUiController?.scoreBoardUIController != null)
                {
                    InGameUiController.scoreBoardUIController.scoreImageElement_Player1.imageSkillIcon.sprite = iconSprite;
                    InGameUiController.scoreBoardUIController.scoreImageElement_Player2.imageSkillIcon.sprite = iconSprite;
                    InGameUiController.scoreBoardUIController.SkillIconImageOnOff(true);
                    InGameUiController.scoreBoardUIController.scoreImageElement_Player1.ChangePlayerImage(4, false, 1);
                    InGameUiController.scoreBoardUIController.scoreImageElement_Player2.ChangePlayerImage(4, false, 2);
                }
                InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[7]);
            }

            CleanupRatEffects();

            DOVirtual.DelayedCall(0.1f, () =>
            {
                FadeImage(0f, 1f);
                IsAnimating = false;

                DOVirtual.DelayedCall(0.9f, () =>
                {
                    if (InGameUiController?.gameManager != null)
                        InGameUiController.gameManager.ResetGame();
                }).SetLink(gameObject);
            }).SetLink(gameObject);
        }).SetLink(gameObject);
    }

    private void CleanupRatEffects()
    {
        if (ratSequence != null) { ratSequence.Kill(); ratSequence = null; }

        if (currentTargetIcon != null) Destroy(currentTargetIcon);
        if (currentHeartObj != null) Destroy(currentHeartObj);
        if (currentRewardIcon != null) Destroy(currentRewardIcon);

        DestroyAllChildren(ransform_RatCardLeft);
        DestroyAllChildren(ransform_RatCardRight);
        DestroyAllChildren(rasnform_RatIconLeft);
        DestroyAllChildren(rasnform_RatIconRight);

        currentTargetIcon = null;
        currentHeartObj = null;
        currentRewardIcon = null;
    }

    private void CreateRatCardAtTransform(Transform targetParent, int skillID)
    {
        if (targetParent == null) return;

        DestroyAllChildren(targetParent);

        GameObject ratCardObj = Instantiate(objSkillCard_Rat, targetParent);
        SkillCard_UI ratCardUI = ratCardObj.GetComponent<SkillCard_UI>();

        ratCardObj.transform.localPosition = Vector3.zero;
        ratCardObj.transform.localRotation = Quaternion.identity;
        ratCardObj.transform.localScale = Vector3.one;

        RectTransform cardRect = ratCardObj.GetComponent<RectTransform>();
        if (cardRect != null)
        {
            cardRect.anchoredPosition = Vector2.zero;
            cardRect.localScale = Vector3.one;
        }

        SkillCard_SO foundData = skillDataList.Find(x => x.iSkillIndex == skillID);

        if (foundData != null)
        {
            ratCardUI.ApplyData(foundData, true);
            ratCardUI.ResetCardAnim();

            if (ratCardUI.text_Description != null)
            {
                ratCardUI.text_Description.DOKill();
                Color visibleColor = ratCardUI.text_Description.color;
                visibleColor.a = 1f;
                ratCardUI.text_Description.color = visibleColor;
            }

            ratCardUI.gameObject.SetActive(true);

            DOVirtual.DelayedCall(0.02f, () =>
            {
                if (ratCardUI != null)
                {
                    Canvas.ForceUpdateCanvases();
                    ratCardUI.StartCardAnimation();
                }
            }).SetLink(ratCardUI.gameObject);
        }
        else
        {
            Debug.LogError($"[RatCard] 스킬 ID {skillID}번을 찾을 수 없습니다!");
        }
    }

    private Vector2 WorldToCanvasPoint(Vector3 worldPosition)
    {
        if (InGameUiController == null || InGameUiController.canvasMain == null) return Vector2.zero;

        Canvas canvas = InGameUiController.canvasMain;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 localPoint;
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : (canvas.worldCamera ?? Camera.main);

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out localPoint);
        return localPoint;
    }

    public void SetBoolAllCardInteract(bool canInteract)
    {
        for (int i = 0; i < instances.Length; i++)
        {
            var card = instances[i];
            if (card == null) continue;
            card.bCanInteract = canInteract;
        }
    }

    public SkillCard_SO FindSkillCardByName(string skillName)
    {
        return skillDataList.Find(card => card.sSkillName == skillName);
    }

    public GameObject CreateSkillInstance(SkillCard_SO card)
    {
        if (card == null) return null;

        int actualIndex = skillDataList.IndexOf(card);
        if (actualIndex < 0 || actualIndex >= skillPrefabs.Count) return null;

        GameObject skillObj = Instantiate(skillPrefabs[actualIndex]);
        Skill skill = skillObj.GetComponent<Skill>();
        if (skill != null) skill.SkillIndex = actualIndex;

        return skillObj;
    }

    public Tween FadeImage(float fTargetAlpha, float duration)
    {
        if (InGameUIController.Instance == null || InGameUIController.Instance.image_FadeOut_White == null) return null;

        if (fTargetAlpha != 0f)
        {
            InGameUIController.Instance.image_FadeOut_White.gameObject.SetActive(true);
        }

        InGameUIController.Instance.image_FadeOut_White.transform.SetAsLastSibling();

        return InGameUIController.Instance.image_FadeOut_White.DOFade(fTargetAlpha, duration)
            .SetLink(InGameUIController.Instance.image_FadeOut_White.gameObject) // [핵심] 안전장치 추가
            .OnComplete(() =>
            {
                if (fTargetAlpha == 0f && InGameUIController.Instance != null && InGameUIController.Instance.image_FadeOut_White != null)
                {
                    InGameUIController.Instance.image_FadeOut_White.gameObject.SetActive(false);
                }
            });
    }

    private void SelectRandomCard()
    {
        List<SkillCard_UI> activeCards = new List<SkillCard_UI>();

        for (int i = 0; i < instances.Length; i++)
        {
            if (instances[i] != null && instances[i].gameObject.activeInHierarchy)
            {
                activeCards.Add(instances[i]);
            }
        }

        if (activeCards.Count > 0)
        {
            int randomIndex = Random.Range(0, activeCards.Count);
            SkillCard_UI selectedCard = activeCards[randomIndex];

            PointerEventData fakeEventData = new PointerEventData(EventSystem.current);
            selectedCard.OnPointerClick(fakeEventData);
        }
    }

    public SkillCard_SO FindSkillCardByIndex(int skillIndex)
    {
        return skillDataList.Find(card => card.iSkillIndex == skillIndex);
    }

    private void MapSkillIcons()
    {
        int ExtractLeadingNumber(string name)
        {
            var match = Regex.Match(name, @"^\d+");
            if (match.Success) return int.Parse(match.Value);
            return -1;
        }

        skillIconMap.Clear();
        foreach (var sprite in sprites_Icons)
        {
            if (sprite == null) continue;
            int number = ExtractLeadingNumber(sprite.name);
            if (number >= 0 && !skillIconMap.ContainsKey(number))
            {
                skillIconMap.Add(number, sprite);
            }
        }
    }

    public Sprite GetSkillIconBySkillIndex(int skillIndex)
    {
        return skillIconMap.ContainsKey(skillIndex) ? skillIconMap[skillIndex] : null;
    }

    private GameObject CreateEffectImage(string objName, Sprite sprite, Vector2 anchoredPos, Vector2 size)
    {
        if (sprite == null || InGameUiController == null || InGameUiController.canvasMain == null) return null;

        GameObject effectObj = new GameObject(objName);
        effectObj.transform.SetParent(InGameUiController.canvasMain.transform, false);
        if (InGameUiController.image_FadeOut_White != null)
            effectObj.transform.SetSiblingIndex(InGameUiController.image_FadeOut_White.transform.GetSiblingIndex() - 1);

        Image img = effectObj.AddComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = false;

        RectTransform rect = effectObj.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;
        rect.localScale = Vector3.one * 1.5f;
        return effectObj;
    }

    public void MarkSkillAsUsed(int skillID)
    {
        if (!usedSkillIDs.Contains(skillID))
        {
            usedSkillIDs.Add(skillID);
        }
    }

    private static void DestroyAllChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            if (child != null) Object.Destroy(child.gameObject);
        }
    }
}