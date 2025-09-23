﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;


[System.Serializable]
public class RatCurtainBoard
{
    public GameObject obj_RatBoard;
    public GameObject obj_curtain;
    public GameObject obj_rat;
    
    [Header("초기 위치 저장")]
    [HideInInspector] public Vector2 originalPos_RatBoard;
    [HideInInspector] public Vector2 originalPos_curtain;
    [HideInInspector] public Vector2 originalPos_rat;

    public void SaveOriginalPositions()
    {
        if (obj_RatBoard != null)
            originalPos_RatBoard = obj_RatBoard.GetComponent<RectTransform>().anchoredPosition;

        if (obj_curtain != null)
            originalPos_curtain = obj_curtain.GetComponent<RectTransform>().anchoredPosition;

        if (obj_rat != null)
            originalPos_rat = obj_rat.GetComponent<RectTransform>().anchoredPosition;
    }

    public void RestoreOriginalPositions()
    {
        if (obj_RatBoard != null)
            obj_RatBoard.GetComponent<RectTransform>().anchoredPosition = originalPos_RatBoard;

        if (obj_curtain != null)
            obj_curtain.GetComponent<RectTransform>().anchoredPosition = originalPos_curtain;

        if (obj_rat != null)
            obj_rat.GetComponent<RectTransform>().anchoredPosition = originalPos_rat;
    }
}



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
    public List<SkillCard_SO> skillDataList = new List<SkillCard_SO>();
    private SkillCard_UI[] instances = new SkillCard_UI[4];

    [Header("카드 생성 위치 배열")]
    [SerializeField] Transform[] spawnPoints = new Transform[4];
    [SerializeField] Transform[] targetPoints = new Transform[4];

    [Header("연출에 사용할 것들")]
    public GameObject[] objs_AnimalSkillCardEffects;

    // 쥐 연출
    public RatCurtainBoard[] ratCurtainBoards;

    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        var datas = Resources.LoadAll<SkillCard_SO>(skillCardResourceFolder);


        for (int i = 0; i < ratCurtainBoards.Length; i++)
        {
            ratCurtainBoards[i].SaveOriginalPositions();
        }



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

            card.ApplyData(null);
            card.gameObject.SetActive(false);
            instances[i] = card;
        }

        // 4개 카드 생성 완료 후 15% 확률로 하나를 쥐 카드로 설정
        //if (Random.Range(0f, 100f) < 15f)
        //{
        //    int randomIndex = Random.Range(0, instances.Length);
        //    instances[randomIndex].bIsRat = true;
        //}


        // 테스트용: 100% 확률로 4개 모두 쥐 카드로 설정
        for (int i = 0; i < instances.Length; i++)
        {
            instances[i].bIsRat = true;
        }




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

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ShowSkillCardListWithSpecific(0, false, new int[] { 16, 103, 108, 24 });
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HIdeSkillCardList_ForRat();
        }

    }
    // 애니메이션 제작할 때 사용하는 테스트용 함수
    public void ShowSkillCardListWithSpecific(int iPlayernum = 0, bool bActivePassive = true, int[] specifiedSkillIndices = null)
    {
        if (skillDataList.Count == 0 || IsAnimating) return;
        iAuthorityPlayerNum = iPlayernum;
        IsAnimating = true;

        List<int> selectedIndices = new List<int>();

        List<int> filteredIndices = new List<int>();
        for (int i = 0; i < skillDataList.Count; i++)
        {
            bool isActive = skillDataList[i].iSkillIndex < 100;
            if ((bActivePassive && isActive) || (!bActivePassive && !isActive))
            {
                filteredIndices.Add(i);
            }
        }

        if (filteredIndices.Count == 0)
        {
            IsAnimating = false;
            return;
        }

        List<int> availableIndices = new List<int>(filteredIndices);

        if (specifiedSkillIndices != null && specifiedSkillIndices.Length > 0)
        {
            foreach (int skillIndex in specifiedSkillIndices)
            {
                if (selectedIndices.Count >= instances.Length) break;

                int foundIndex = -1;
                for (int i = 0; i < skillDataList.Count; i++)
                {
                    if (skillDataList[i].iSkillIndex == skillIndex)
                    {
                        foundIndex = i;
                        break;
                    }
                }

                if (foundIndex >= 0)
                {
                    selectedIndices.Add(foundIndex);
                    availableIndices.Remove(foundIndex);
                }
            }
        }

        for (int i = selectedIndices.Count; i < instances.Length && availableIndices.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedIdx = availableIndices[randomIndex];
            selectedIndices.Add(selectedIdx);
            availableIndices.RemoveAt(randomIndex);
        }

        StartShowingCards(selectedIndices);
    }





    // #. 스킬 보여주는 함수
    public void ShowSkillCardList(int iPlayernum = 0, bool bActivePassive = true, int[] iCardArray = null)
    {
        if (skillDataList.Count == 0 || IsAnimating) return;
        iAuthorityPlayerNum = iPlayernum;
        IsAnimating = true;

        // 현재 플레이어의 PlayerAbility 가져오기
        PlayerAbility currentPlayerAbility = null;
        if (InGameUiController?.gameManager != null)
        {
            if (iPlayernum == 1)
                currentPlayerAbility = InGameUiController.gameManager.playerAbility_1;
            else if (iPlayernum == 2)
                currentPlayerAbility = InGameUiController.gameManager.playerAbility_2;
        }

        List<int> selectedIndices = new List<int>();

        // iCardArray가 있으면 그 값들을 사용
        if (iCardArray != null && iCardArray.Length > 0)
        {
            for (int i = 0; i < instances.Length && i < iCardArray.Length; i++)
            {
                int cardIndex = Mathf.Clamp(iCardArray[i], 0, skillDataList.Count - 1);
                selectedIndices.Add(cardIndex);
            }

            StartShowingCards(selectedIndices);
            // 외부에서 받은 데이터라도 약간의 딜레이 추가
            //DOVirtual.DelayedCall(0.05f, () => {

            //});
        }
        else
        {
            // 조건에 맞는 스킬만 필터링
            List<int> filteredIndices = new List<int>();
            for (int i = 0; i < skillDataList.Count; i++)
            {
                bool isActive = skillDataList[i].iSkillIndex < 100;
                if ((bActivePassive && isActive) || (!bActivePassive && !isActive))
                {
                    filteredIndices.Add(i);
                }
            }

            // 사용 가능한 스킬이 없으면 종료
            if (filteredIndices.Count == 0)
            {
                IsAnimating = false;
                return;
            }



            List<int> availableIndices = new List<int>(filteredIndices);

            // 이미 보유한 스킬 제거
            for (int i = availableIndices.Count - 1; i >= 0; i--)
            {
                if (IsSkillOwned(currentPlayerAbility, availableIndices[i]))
                {
                    availableIndices.RemoveAt(i);
                }
            }

            // 사용 가능한 스킬이 부족하면 원래 리스트 사용
            if (availableIndices.Count < instances.Length)
            {
                availableIndices = new List<int>(filteredIndices);
            }

            // 중복 없이 선택
            for (int i = 0; i < instances.Length && availableIndices.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, availableIndices.Count);
                int selectedIdx = availableIndices[randomIndex];
                selectedIndices.Add(selectedIdx);
                availableIndices.RemoveAt(randomIndex); // 선택된 것은 제거
            }



            // 메시지 전송 후 약간의 딜레이를 두고 카드 표시
            P2PMessageSender.SendMessage(
                SkillShowBuilder.Build(MatchResultStore.myPlayerNumber, selectedIndices.ToArray())
            );

            StartShowingCards(selectedIndices);
            // 메시지 전송 후 짧은 딜레이로 동기화 개선
            //DOVirtual.DelayedCall(0.05f, () => {

            //});
        }
    }



    // 실제 카드 표시 로직을 분리한 함수
    private void StartShowingCards(List<int> selectedIndices)
    {
        int completed = 0;
        int total = instances.Length;
        bool hasError = false;


        FadeImage(1f, 0f).OnComplete(() =>
        {
            InGameUiController.scoreBoardUIController.ActiveFalseBones();


            text_Timer.gameObject.SetActive(true);
            iTimerForSelect = 15;
            fTimerInternal = 15.0f;
            bTimerCheck = true;
            text_Timer.text = iTimerForSelect.ToString();


            for (int i = 0; i < total; i++)
            {
                var card = instances[i];
                if (card == null || targetPoints[i] == null)
                {
                    completed++;
                    if (completed >= total && !hasError)
                    {
                        CompleteCardShow();
                    }
                    continue;
                }

                if (i < selectedIndices.Count)
                {
                    int idx = selectedIndices[i];
                    try
                    {
                        card.ApplyData(skillDataList[idx]);
                        card.ResetCardAnim();
                        card.gameObject.SetActive(true);
                        card.StartCardAnimation();

                        var currentCard = card;
                        card.transform.DOMove(targetPoints[i].position, 0.3f)
                            .OnComplete(() =>
                            {
                                if (!hasError)
                                {
                                    currentCard.StartFloatingAnimation();
                                    completed++;
                                    if (completed >= total)
                                    {
                                        CompleteCardShow();
                                    }
                                }
                            })
                            .OnKill(() =>
                            {
                                // Tween이 중단된 경우에도 completed 증가
                                if (!hasError)
                                {
                                    completed++;
                                    if (completed >= total)
                                    {
                                        CompleteCardShow();
                                    }
                                }
                            });
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[SkillCardController] Error showing card {i}: {e.Message}");
                        hasError = true;
                        IsAnimating = false;
                        return;
                    }
                }
                else
                {
                    completed++;
                    if (completed >= total && !hasError)
                    {
                        CompleteCardShow();
                    }
                }
            }




            DOVirtual.DelayedCall(0.1f, () =>
            {
                FadeImage(0f, 1f);
            });
        });
    }

    // 카드 표시 완료 처리를 별도 함수로 분리
    private void CompleteCardShow()
    {
        IsAnimating = false;
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

      


    public void HideSkillCardList(int iAnimalNum = 0, Vector2 clickedCardPosition = default)
    {
        if (IsAnimating) return;
        IsAnimating = true;
        SetBoolAllCardInteract(false);

       
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
            text_Timer.gameObject.SetActive(false) ;
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
        iAuthorityPlayerNum = 0;
        if (iAnimalNum >= 0 && iAnimalNum < objs_AnimalSkillCardEffects.Length)
        {
            GameObject effectObj = Instantiate(objs_AnimalSkillCardEffects[iAnimalNum], InGameUiController.canvasMain.transform);
            RectTransform effectRect = effectObj.GetComponent<RectTransform>();

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
                        DOVirtual.DelayedCall(0.1f, () =>
                        {
                            FadeImage(0f, 1f);
                            IsAnimating = false;
                            DOVirtual.DelayedCall(0.9f, () =>
                            {
                                InGameUiController.scoreBoardUIController.OpenScorePanel();
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
                InGameUiController.scoreBoardUIController.OpenScorePanel();
            });
        }
    }

    public void HIdeSkillCardList_ForRat(int iAnimalNum = 0, Vector2 clickedCardPosition = default)
    {
        if (IsAnimating) return;
        IsAnimating = true;
        SetBoolAllCardInteract(false);
        clickedCardPosition = new Vector2(-750, 330);
        int ratBoardIndex = clickedCardPosition.x < 0 ? 0 : 1;
        if (ratBoardIndex < ratCurtainBoards.Length)
        {
            ratCurtainBoards[ratBoardIndex].RestoreOriginalPositions();

            if (ratCurtainBoards[ratBoardIndex].obj_RatBoard != null)
                ratCurtainBoards[ratBoardIndex].obj_RatBoard.SetActive(true);
        }
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
        iAuthorityPlayerNum = 0;
        if (iAnimalNum >= 0 && iAnimalNum < objs_AnimalSkillCardEffects.Length)
        {
            // 커튼 애니메이션 추가
            if (ratBoardIndex < ratCurtainBoards.Length && ratCurtainBoards[ratBoardIndex].obj_curtain != null)
            {
                RectTransform curtainRect = ratCurtainBoards[ratBoardIndex].obj_curtain.GetComponent<RectTransform>();
                Vector2 originalCurtainPos = curtainRect.anchoredPosition;
                Vector2 targetCurtainPos = new Vector2(originalCurtainPos.x, originalCurtainPos.y + 1500);

                // 1.2초 딜레이 후 커튼 애니메이션 시작
                DOVirtual.DelayedCall(1.2f, () =>
                {
                    Sequence curtainSequence = DOTween.Sequence();
                    curtainSequence.Append(curtainRect.DOAnchorPosY(originalCurtainPos.y - 50f, 0.3f));
                    curtainSequence.Append(curtainRect.DOAnchorPos(targetCurtainPos, 0.9f).SetEase(Ease.OutQuad));
                    curtainSequence.OnComplete(() =>
                    {
                        // 커튼 애니메이션 완료 후 1초 대기 후 기존 로직 실행
                        DOVirtual.DelayedCall(1f, () =>
                        {
                            ExecuteOriginalEffect();
                        });
                    });
                });
            }
            else
            {
                // 커튼이 없으면 바로 기존 로직 실행
                ExecuteOriginalEffect();
            }

            void ExecuteOriginalEffect()
            {
                GameObject effectObj = Instantiate(objs_AnimalSkillCardEffects[iAnimalNum], InGameUiController.canvasMain.transform);
                RectTransform effectRect = effectObj.GetComponent<RectTransform>();
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
                            DOVirtual.DelayedCall(0.1f, () =>
                            {
                                FadeImage(0f, 1f);
                                IsAnimating = false;
                                DOVirtual.DelayedCall(0.9f, () =>
                                {
                                    InGameUiController.scoreBoardUIController.OpenScorePanel();
                                });
                            });
                        });
                        Destroy(effectObj);
                    });
                });
            }
        }
        else
        {
            IsAnimating = false;
            DOVirtual.DelayedCall(1f, () =>
            {
                InGameUiController.scoreBoardUIController.OpenScorePanel();
            });
        }
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

            // ��¥ Ŭ�� �̺�Ʈ �����ؼ� OnPointerClick ȣ��
            PointerEventData fakeEventData = new PointerEventData(EventSystem.current);
            selectedCard.OnPointerClick(fakeEventData);
        }
    }

}
