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

    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        var datas = Resources.LoadAll<SkillCard_SO>(skillCardResourceFolder);



        // 숫자 추출 함수
        int ExtractLeadingNumber(string name)
        {
            var match = Regex.Match(name, @"^\d+");
            if (match.Success)
                return int.Parse(match.Value);
            return int.MaxValue; // 숫자 없으면 맨 뒤로
        }

        var sortedDatas = datas.OrderBy(d => ExtractLeadingNumber(d.name)).ToArray();

        // 0번 데이터 제외하고 추가
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

        // 2. 10% 확률 체크
        if (Random.Range(0f, 100f) < 10f)
        {
            // 3. 당첨되면 인스턴스 중 하나를 랜덤으로 골라 쥐로 설정
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



    // #. 스킬 보여주는 함수


    public void ShowSkillCardList(int iPlayernum = 0, bool bActivePassive = true, int[] iCardArray = null)
    {
        if (skillDataList.Count == 0 || IsAnimating) return;

        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[6]);
        iAuthorityPlayerNum = iPlayernum;
        IsAnimating = true;

        PlayerAbility currentPlayerAbility = null;
        // opponentPlayerAbility 변수는 이제 필터링에서 쓰지 않으므로 가져올 필요 없지만, 구조 유지를 위해 둠 (혹은 삭제해도 됨)

        if (InGameUiController?.gameManager != null)
        {
            if (iPlayernum == 1)
            {
                currentPlayerAbility = InGameUiController.gameManager.playerAbility_1;
            }
            else if (iPlayernum == 2)
            {
                currentPlayerAbility = InGameUiController.gameManager.playerAbility_2;
            }
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
            // 1. 기초 필터링 (제외 리스트 + 사용 리스트 배제)
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

            // 2. 가용 후보군 설정
            List<int> availableIndices = new List<int>(filteredIndices);

            // 3. 이미 보유한 스킬 제거 (중복 방지) - [수정됨: 상대방 스킬 체크 삭제]
            for (int i = availableIndices.Count - 1; i >= 0; i--)
            {
                int currentID = skillDataList[availableIndices[i]].iSkillIndex;

                // [수정] 오직 '내 캐릭터'가 가진 스킬인지만 확인합니다.
                if (IsSkillOwned(currentPlayerAbility, currentID))
                {
                    availableIndices.RemoveAt(i);
                }
            }

            // 4. [1차 보충] 후보가 부족하면 "보유 중인 스킬"이라도 가져옴
            if (availableIndices.Count < instances.Length)
            {
                foreach (int idx in filteredIndices)
                {
                    if (!availableIndices.Contains(idx))
                    {
                        availableIndices.Add(idx);
                    }
                    if (availableIndices.Count >= instances.Length) break;
                }
            }

            // 5. 랜덤 선택
            for (int i = 0; i < instances.Length && availableIndices.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
                int selectedIdx = availableIndices[randomIndex];
                selectedIndices.Add(selectedIdx);
                availableIndices.RemoveAt(randomIndex);
            }

            // 6. [최후의 보루] 그래도 부족하면 "타입 맞는 안전한 스킬" 중에서 중복 허용 채움
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
                else
                {
                    Debug.LogError("[ShowSkillCardList] 해당 타입의 안전한 스킬 데이터가 아예 없습니다!");
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

        // 데이터가 모자란 경우를 대비해 total과 indices 개수 중 작은 쪽을 기준으로 하거나, 
        // 로직상 무조건 instances만큼 돌리되 데이터가 없으면 빈 껍데기 처리
        // (작성하신 코드는 인덱스가 모자라면 뒤쪽 카드를 안 켜는 구조입니다)

        // 1. 화면을 즉시 흰색으로 가림
        FadeImage(1f, 0.2f).OnComplete(() =>
        {
            // 화면 페이드 인 예약
            DOVirtual.DelayedCall(0.1f, () =>
            {
                FadeImage(0f, 0.5f);
            });

            // UI 정리
            if (InGameUiController?.scoreBoardUIController != null)
            {
                InGameUiController.scoreBoardUIController.ActiveFalseBones();
                InGameUiController.scoreBoardUIController.OnOffScoreTextObj(false);
            }

            // 타이머 설정
            if (text_Timer != null)
            {
                text_Timer.gameObject.SetActive(true);
                iTimerForSelect = 15;
                fTimerInternal = 15.0f;
                bTimerCheck = true;
                text_Timer.text = iTimerForSelect.ToString();
            }

            // =========================================================
            // [수정] try-catch를 for문 내부로 이동
            // =========================================================
            for (int i = 0; i < total; i++)
            {
                try
                {
                    var card = instances[i];

                    // [문제 의심 1] Inspector에서 참조가 빠져있으면 에러 없이 그냥 "스킵"됨 -> 2,3개만 나오는 원인 가능성
                    if (card == null || targetPoints[i] == null)
                    {
                        Debug.LogError($"[SkillCardController] {i}번 인덱스의 Card UI 또는 TargetPoint가 연결되지 않았습니다!");
                        completed++;
                        if (completed >= total) CompleteCardShow();
                        continue;
                    }

                    if (i < selectedIndices.Count)
                    {
                        int idx = selectedIndices[i];

                        // 데이터 적용
                        card.ApplyData(skillDataList[idx], false);
                        card.ResetCardAnim();
                        card.gameObject.SetActive(true);

                        // [문제 의심 2] 여기서 내부 에러(애니메이션 등) 터지면 catch로 가서 다음 카드로 넘어감 (이제 멈추지 않음)
                        card.StartCardAnimation();

                        var currentCard = card;
                        card.transform.DOMove(targetPoints[i].position, 0.3f)
                            .OnComplete(() =>
                            {
                                currentCard.StartFloatingAnimation();
                                completed++;
                                if (completed >= total) CompleteCardShow();
                            })
                            // OnKill은 트윈이 강제 종료될 때 호출됨 (안전장치)
                            .OnKill(() =>
                            {
                                // OnComplete가 호출 안 된 상태에서 죽었을 때만 카운트해야 함
                                // 하지만 로직 복잡성을 피하기 위해 중복 호출 방지는 CompleteCardShow 내부나 completed 변수 관리로 해결 추천
                                // 여기서는 단순 카운팅 유지
                                if (currentCard.gameObject.activeSelf) // 켜져있는 상태에서 죽었으면 카운트
                                {
                                    // 중복 카운팅 이슈가 있을 수 있으나, 멈추는 것보단 나음
                                }
                            });
                    }
                    else
                    {
                        // selectedIndices 개수가 instances보다 적으면 나머지는 스킵
                        // "스킬이 부족할 일은 절대 없다"고 하셨지만, 만약 이 분기 탄다면 코드가 데이터를 못 가져온 것
                        Debug.LogWarning($"[SkillCardController] {i}번 카드를 위한 스킬 데이터가 부족합니다.");
                        completed++;
                        if (completed >= total) CompleteCardShow();
                    }
                }
                catch (System.Exception e)
                {
                    // ★ 핵심: 3번 카드가 터져도 4번 카드는 그린다.
                    Debug.LogError($"[SkillCardController] {i}번 카드 출력 중 치명적 오류 발생: {e.Message}\n{e.StackTrace}");

                    // 에러가 났어도 카운트는 올려야 게임이 진행됨
                    completed++;
                    if (completed >= total) CompleteCardShow();
                }
            }
        });
    }



    // 카드 표시 완료 처리를 별도 함수로 분리
    private void CompleteCardShow()
    {
        IsAnimating = false;

        // #. 테스트용임 수정 꼭 해야함
        //if (Random.Range(0f, 100f) < 15f)
        //{
        //    int randomIndex = Random.Range(0, instances.Length);
        //    instances[randomIndex].bIsRat = true;
        //}
        //for (int i = 0; i < instances.Length; i++)  instances[i].bIsRat = true;
 

        SetBoolAllCardInteract(true);
    }


    // #. 이미 보유하고 있는 스킬인지 판단하는 함수
    private bool IsSkillOwned(PlayerAbility playerAbility, int skillIndex)
    {
        if (playerAbility == null) return false;

        if (playerAbility.skill1 != null && playerAbility.skill1.SkillIndex == skillIndex)
            return true;
        if (playerAbility.skill2 != null && playerAbility.skill2.SkillIndex == skillIndex)
            return true;

        return false;
    }



    public void HideSkillCardList(int iSkillIndex = 0, Vector2 clickedCardPosition = default)
    {
        if (IsAnimating) return;
        IsAnimating = true;
        SetBoolAllCardInteract(false);


        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[9]);


        // floating 애니메이션 정지
        for (int i = 0; i < instances.Length; i++)
        {
            var card = instances[i];
            if (card != null)
            {
                card.StopFloatingAnimation();
            }
        }

        FadeImage(1f, 0f).OnComplete(() =>
        {
            text_Timer.gameObject.SetActive(false);
            iTimerForSelect = 0;
            fTimerInternal = 0f;
            bTimerCheck = false;

            DOVirtual.DelayedCall(0.1f, () =>
            {
                FadeImage(0f, 1f);
            });
        });
        int total = instances.Length;
        for (int i = 0; i < total; i++)
        {
            var card = instances[i];
            if (card == null || spawnPoints[i] == null)
            {
                continue;
            }
            card.transform.position = spawnPoints[i].position; // 즉시 이동
            card.gameObject.SetActive(false);
        }

        int iTemp = iAuthorityPlayerNum;
        iAuthorityPlayerNum = 0;

        SkillCard_SO selectedSkillData = skillDataList.Find(data => data.iSkillIndex == iSkillIndex);

        if (selectedSkillData != null)
        {
            // 2. 실제 스킬 인덱을 사용하여 매핑된 아이콘 스프라이트를 가져옴
            Sprite skillIconSprite = GetSkillIconBySkillIndex(selectedSkillData.iSkillIndex);

            if (skillIconSprite != null)
            {
                // 3. GameObject 대신 Image 컴포넌트를 가진 새로운 오브젝트를 생성
                GameObject effectObj = new GameObject("SkillIconEffect");
                RectTransform effectRect = effectObj.AddComponent<RectTransform>();
                Image effectImage = effectObj.AddComponent<Image>();
                effectImage.sprite = skillIconSprite;
                effectRect.sizeDelta = new Vector2(200f, 200f); // 스프라이트 크기에 맞춤
                effectImage.raycastTarget = false; // 클릭 방지

                effectRect.SetParent(InGameUiController.canvasMain.transform, false);


                // effectObj의 크기를 조절 (선택 사항)
                effectRect.localScale = Vector3.one * 1.5f;

                // (이하 기존 로직 유지)
                // image_FadeOut_White보다 뒤에 보이게 설정
                effectRect.SetSiblingIndex(InGameUIController.Instance.image_FadeOut_White.transform.GetSiblingIndex() - 1);

                // 클릭한 카드 위치 사용
                effectRect.anchoredPosition = clickedCardPosition;

                DOVirtual.DelayedCall(1f, () =>
            {
                effectRect.DOAnchorPos(Vector2.zero, 0.6f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    FadeImage(1f, 0f).OnComplete(() =>
                    {
                        InGameUiController.scoreBoardUIController.scoreImageElement_Player1.ChangePlayerImage(4, false, 1);
                        InGameUiController.scoreBoardUIController.scoreImageElement_Player2.ChangePlayerImage(4, false, 2);

                        Sprite skillIconSprite = GetSkillIconBySkillIndex(iSkillIndex);
                        InGameUiController.scoreBoardUIController.scoreImageElement_Player1.imageSkillIcon.sprite = skillIconSprite;
                        InGameUiController.scoreBoardUIController.scoreImageElement_Player2.imageSkillIcon.sprite = skillIconSprite;
                        InGameUiController.scoreBoardUIController.SkillIconImageOnOff(true);

                        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[7]);

                        DOVirtual.DelayedCall(0.1f, () =>
                        {
                            FadeImage(0f, 1f);
                            IsAnimating = false;
                            DOVirtual.DelayedCall(0.9f, () =>
                            {
                                InGameUiController.gameManager.ResetGame();

                                DOVirtual.DelayedCall(0.6f, () =>
                                {
                                    // InGameUiController.scoreBoardUIController.OpenScorePanel();
                                });

                            });
                        });
                    });
                    Destroy(effectObj);
                });
            });
            }
            else
            {
                IsAnimating = false;
                DOVirtual.DelayedCall(1f, () =>
                {

                    InGameUiController.gameManager.ResetGame();

                    DOVirtual.DelayedCall(0.6f, () =>
                    {
                        // InGameUiController.scoreBoardUIController.OpenScorePanel();
                    });
                });
            }
        }
    }

    // -----------------------------------------------------------------------
    // 🐭 쥐 연출 함수 (RatCurtainBoard 없이 Transform 사용)
    // -----------------------------------------------------------------------
    public void HIdeSkillCardList_ForRat(int iSkillIndex, Vector3 clickedWorldPosition, int iRaySkillIndex)
    {
        if (IsAnimating) return;

        CleanupRatEffects();
        IsAnimating = true;
        SetBoolAllCardInteract(false);
        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[9]);

        // 클릭 지점 (타겟 아이콘이 처음 생길 위치)
        Vector2 uiAnchoredPos = WorldToCanvasPoint(clickedWorldPosition);

        ratSequence = DOTween.Sequence();

        // --- [Step 1] 초기 페이드 및 UI 정리 ---
        ratSequence.AppendCallback(() =>
        {
            FadeImage(1f, 0f).OnComplete(() =>
            {
                text_Timer.gameObject.SetActive(false);
                iTimerForSelect = 0;
                fTimerInternal = 0f;
                bTimerCheck = false;
                // 페이드 인은 시퀀스와 별개로 즉시 실행되지 않도록 주의 (여기서는 시각적 효과만)
                DOVirtual.DelayedCall(0.1f, () => FadeImage(0f, 1f));
            });

            // 기존 카드 숨기기
            for (int i = 0; i < instances.Length; i++)
            {
                if (instances[i] != null) instances[i].gameObject.SetActive(false);
            }
        });

        // --- [Step 2] 타겟 아이콘 생성 ---
        ratSequence.AppendCallback(() =>
        {
            Sprite skillIconSprite = GetSkillIconBySkillIndex(iSkillIndex);
            currentTargetIcon = CreateEffectImage("TargetSkillIcon", skillIconSprite, uiAnchoredPos, new Vector2(200f, 200f));

            if (currentTargetIcon != null)
            {
                RectTransform targetIconRect = currentTargetIcon.GetComponent<RectTransform>();
                targetIconRect.SetParent(InGameUiController.image_FadeOut_White.transform.parent);
                InGameUiController.image_FadeOut_White.transform.SetAsLastSibling();
            }
        });

        // 0.5초 대기
        ratSequence.AppendInterval(0.5f);

        // --- [Step 3] 하트 미사일 생성 및 발사 ---
        ratSequence.AppendCallback(() =>
        {
            float canvasHeight = InGameUiController.canvasMain.GetComponent<RectTransform>().rect.height;
            Vector2 startHeartPos = new Vector2(0f, canvasHeight / 2f + 300f);

            currentHeartObj = CreateEffectImage("RatHeartMissile", sprite_RatHeart, startHeartPos, new Vector2(150f, 150f));

            if (currentHeartObj != null)
            {
                RectTransform heartRect = currentHeartObj.GetComponent<RectTransform>();
                heartRect.SetParent(InGameUiController.image_FadeOut_White.transform.parent);
                InGameUiController.image_FadeOut_White.transform.SetAsLastSibling();

                // 하트 애니메이션 (시퀀스에 Insert하지 않고 자체적으로 돔, 하지만 관리를 위해 변수에 할당)
                heartRect.DORotate(new Vector3(0, 0, 360 * 2), 0.6f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
                heartRect.DOAnchorPos(uiAnchoredPos, 0.6f).SetEase(Ease.InQuad);
            }
        });

        // 0.6초 대기 (하트 이동 시간)
        ratSequence.AppendInterval(0.6f);

        // --- [Step 4] 아이콘 추락 ---
        ratSequence.AppendCallback(() =>
        {
            float canvasHeight = InGameUiController.canvasMain.GetComponent<RectTransform>().rect.height;
            if (currentTargetIcon != null)
            {
                RectTransform targetIconRect = currentTargetIcon.GetComponent<RectTransform>();
                targetIconRect.DORotate(new Vector3(0, 0, 720), 0.8f, RotateMode.FastBeyond360);
                targetIconRect.DOAnchorPosY(-canvasHeight, 0.8f).SetEase(Ease.InBack);
            }
        });

        // 0.8초 대기 (추락 시간) + 0.7초 여유 = 1.5초
        ratSequence.AppendInterval(1.5f);

        // --- [Step 5] 화면 전환 및 쥐 카드 생성 ---
        ratSequence.AppendCallback(() =>
        {
            if (currentTargetIcon != null) Destroy(currentTargetIcon);
            if (currentHeartObj != null) Destroy(currentHeartObj);

            FadeImage(1f, 0.1f).OnComplete(() =>
            {
                bool isRight = (uiAnchoredPos.x > 0);
                int realID = iRaySkillIndex;

                // 쥐 카드 생성
                CreateRatCardAtTransform(
                    isRight ? ransform_RatCardRight : ransform_RatCardLeft,
                    realID
                );

                // 보상 아이콘 생성
                Sprite rewardIcon = GetSkillIconBySkillIndex(realID);

                currentRewardIcon = CreateEffectImage(
                    "RewardSkillIcon",
                    rewardIcon,
                    Vector2.zero,               // 일단 0
                    new Vector2(200f, 200f)
                );

                if (currentRewardIcon != null)
                {
                    RectTransform rewardRect = currentRewardIcon.GetComponent<RectTransform>();

                    // 🔥 핵심: Canvas 기준으로 고정 좌표 지정
                    rewardRect.SetParent(InGameUiController.canvasMain.transform, false);

                    rewardRect.anchoredPosition = isRight
                        ? new Vector2(457f, 410f)
                        : new Vector2(-457f, 410f);

                    rewardRect.localScale = Vector3.one * 1.4f;
                }

                DOVirtual.DelayedCall(0.1f, () => FadeImage(0f, 0.5f));
            });
        });

        // 4.3초 대기 (쥐 카드 보여주는 시간)
        ratSequence.AppendInterval(4.4f); // 페이드 시간 고려하여 약간 넉넉하게

        // --- [Step 6] 최종 아이콘 획득 및 게임 리셋 ---
        ratSequence.AppendCallback(() =>
        {
            if (currentRewardIcon != null)
            {
                RectTransform iconRect = currentRewardIcon.GetComponent<RectTransform>();
                iconRect.DOAnchorPos(Vector2.zero, 0.6f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    FinishRatSequence(true);
                });
            }
            else
            {
                // 아이콘이 없어도 게임은 진행되어야 함
                FinishRatSequence(false);
            }
        });
    }

    private void FinishRatSequence(bool hasIcon)
    {
        FadeImage(1f, 0f).OnComplete(() =>
        {
            // 점수판 이미지 변경 등 UI 업데이트
            if (hasIcon && currentRewardIcon != null)
            {
                Sprite iconSprite = currentRewardIcon.GetComponent<Image>().sprite;
                InGameUiController.scoreBoardUIController.scoreImageElement_Player1.imageSkillIcon.sprite = iconSprite;
                InGameUiController.scoreBoardUIController.scoreImageElement_Player2.imageSkillIcon.sprite = iconSprite;
                InGameUiController.scoreBoardUIController.SkillIconImageOnOff(true);

                InGameUiController.scoreBoardUIController.scoreImageElement_Player1.ChangePlayerImage(4, false, 1);
                InGameUiController.scoreBoardUIController.scoreImageElement_Player2.ChangePlayerImage(4, false, 2);
                InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[7]);
            }

            // 생성된 임시 오브젝트들 제거
            CleanupRatEffects();

            DOVirtual.DelayedCall(0.1f, () =>
            {
                FadeImage(0f, 1f);
                IsAnimating = false;

                DOVirtual.DelayedCall(0.9f, () =>
                {
                    InGameUiController.gameManager.ResetGame();
                });
            });
        });
    }

    // [핵심] 쥐 연출 관련 모든 트윈과 오브젝트를 강제 정리하는 함수
    private void CleanupRatEffects()
    {
        if (ratSequence != null) { ratSequence.Kill(); ratSequence = null; }

        if (currentTargetIcon != null) Destroy(currentTargetIcon);
        if (currentHeartObj != null) Destroy(currentHeartObj);
        if (currentRewardIcon != null) Destroy(currentRewardIcon);

        // RectTransform 자식들 정리
        if (ransform_RatCardLeft != null) foreach (Transform child in ransform_RatCardLeft) Destroy(child.gameObject);
        if (ransform_RatCardRight != null) foreach (Transform child in ransform_RatCardRight) Destroy(child.gameObject);

        // 추가된 RectTransform 포인트들도 정리 (필요 시)
        if (rasnform_RatIconLeft != null) foreach (Transform child in rasnform_RatIconLeft) Destroy(child.gameObject);
        if (rasnform_RatIconRight != null) foreach (Transform child in rasnform_RatIconRight) Destroy(child.gameObject);

        currentTargetIcon = null;
        currentHeartObj = null;
        currentRewardIcon = null;
    }


    private void CreateRatCardAtTransform(Transform targetParent, int skillID)
    {
        if (targetParent == null) return;

        foreach (Transform child in targetParent) Destroy(child.gameObject);

        GameObject ratCardObj = Instantiate(objSkillCard_Rat, targetParent);
        SkillCard_UI ratCardUI = ratCardObj.GetComponent<SkillCard_UI>();

        // 1. 위치/크기 강제 초기화
        ratCardObj.transform.localPosition = Vector3.zero;
        ratCardObj.transform.localRotation = Quaternion.identity;
        ratCardObj.transform.localScale = Vector3.one;

        RectTransform cardRect = ratCardObj.GetComponent<RectTransform>();
        if (cardRect != null)
        {
            cardRect.anchoredPosition = Vector2.zero;
            cardRect.localScale = Vector3.one;
        }

        // 2. [핵심 수정] 리스트 순서(Index)가 아닌 스킬 번호(ID)로 찾습니다.
        // 기존: foundData = skillDataList[listIndex]; (여기서 8을 넣으면 9가 나옴)
        // 변경: Find 함수로 ID가 일치하는 녀석을 직접 찾음
        SkillCard_SO foundData = skillDataList.Find(x => x.iSkillIndex == skillID);

        if (foundData != null)
        {
            // 3. 데이터 적용
            ratCardUI.ApplyData(foundData, true);

            // ... (4. 일러스트 그룹 강제 교정 코드 생략) ...

            // 5. 리셋 및 애니메이션
            ratCardUI.ResetCardAnim();

            // ▼▼▼ [추가된 코드] 쥐 연출 카드는 설명 텍스트 무조건 보이기 ▼▼▼
            if (ratCardUI.text_Description != null)
            {
                // ResetCardAnim에서 실행된 FadeOut 트윈을 죽임
                ratCardUI.text_Description.DOKill(); 
                
                // 알파값을 1로 강제 변경
                Color visibleColor = ratCardUI.text_Description.color;
                visibleColor.a = 1f;
                ratCardUI.text_Description.color = visibleColor;
            }
            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            ratCardUI.gameObject.SetActive(true);

            DOVirtual.DelayedCall(0.02f, () =>
            {
                if (ratCardUI != null)
                {
                    Canvas.ForceUpdateCanvases();
                    ratCardUI.StartCardAnimation();
                }
            });
        }
        else
        {
            Debug.LogError($"[RatCard] 스킬 ID {skillID}번을 찾을 수 없습니다!");
        }

    }


    private Vector2 WorldToCanvasPoint(Vector3 worldPosition)
    {
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
        Debug.Log("전원 " + canInteract + "로 설정됨");

        for (int i = 0; i < instances.Length; i++)
        {
            var card = instances[i];
            if (card == null) continue;
            if (canInteract)
                card.bCanInteract = true;
            else
                card.bCanInteract = false;
        }
    }


    // #. Skill 이름으로 찾아와서 카드 생성하기
    public SkillCard_SO FindSkillCardByName(string skillName)
    {
        return skillDataList.Find(card => card.sSkillName == skillName);
    }


    // #. SkillCard 인스턴스 생성
    public GameObject CreateSkillInstance(SkillCard_SO card)
    {
        if (card == null)
        {
            Debug.LogWarning("[SkillCardController] SkillCard_SO is null.");
            return null;
        }

        // skillDataList에서 해당 카드의 실제 인덱스 찾기
        int actualIndex = skillDataList.IndexOf(card);
        if (actualIndex < 0)
        {
            Debug.LogError($"[SkillCardController] Card '{card.name}' not found in skillDataList.");
            return null;
        }

        if (actualIndex >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillCardController] actualIndex({actualIndex}) is out of range. skillPrefabs.Count = {skillPrefabs.Count}");
            return null;
        }

        GameObject skillObj = Instantiate(skillPrefabs[actualIndex]);
        Skill skill = skillObj.GetComponent<Skill>();
        if (skill != null)
        {
            skill.SkillIndex = actualIndex;  // 인덱스 설정
        }

        return skillObj;
    }


    public Tween FadeImage(float fTargetAlpha, float duration)
    {
        // 시작할 때, 목표 알파값이 0이 아니라면 활성화
        if (fTargetAlpha != 0f)
        {
            InGameUIController.Instance.image_FadeOut_White.gameObject.SetActive(true);
        }

        InGameUIController.Instance.image_FadeOut_White.transform.SetAsLastSibling();
        return InGameUIController.Instance.image_FadeOut_White.DOFade(fTargetAlpha, duration)
            .OnComplete(() =>
            {
                // 마지막 알파값이 0이라면 비활성화
                if (fTargetAlpha == 0f)
                {
                    InGameUIController.Instance.image_FadeOut_White.gameObject.SetActive(false);
                }
            });
    }


    // #. 시간 초과되면 랜덤으로 선택되게 하는 함수
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


    private void OnRatAnimationComplete()
    {
        FadeImage(1f, 0f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                FadeImage(0f, 1f);
                IsAnimating = false;
                DOVirtual.DelayedCall(0.9f, () =>
                {
                    InGameUiController.gameManager.ResetGame();
                    //DOVirtual.DelayedCall(0.6f, () =>
                    //{
                    //    InGameUiController.scoreBoardUIController.OpenScorePanel();
                    //});
                });
            });
        });
    }


    private GameObject CreateEffectImage(string objName, Sprite sprite, Vector2 anchoredPos, Vector2 size)
    {
        if (sprite == null) return null;
        GameObject effectObj = new GameObject(objName);
        effectObj.transform.SetParent(InGameUiController.canvasMain.transform, false);
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



}



