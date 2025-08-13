using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;



// 메세지 종류 3개만 보내면 될듯

// 점수판 보여주기
// 스킬 선택창 보여주기
// 스킬 골랐음

// 점수판 보여주기
// 스킬 선택창 보여주기
// 스킬 골랐음

// 점수판 보여주기
// 스킬 선택창 보여주기
// 스킬 골랐음

// 점수판 보여주기
// 스킬 선택창 보여주기
// 스킬 골랐음





public class SkillCardController : MonoBehaviour 
{
    [Header("중요한 정보들")]
    public int iAuthorityPlayerNum = 0;
    public InGameUIController InGameUiController { get; set; }
    public bool IsAnimating { get; private set; }
   

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

            card.ApplyData(null);
            card.gameObject.SetActive(false);
            instances[i] = card;
        }
    }



    public void ShowSkillCardList(int iPlayernum = 0)
    {
        if (skillDataList.Count == 0 || IsAnimating) return;
        iAuthorityPlayerNum = iPlayernum;
        IsAnimating = true;
        int completed = 0;
        int total = instances.Length;

        List<int> randomIndices = new List<int>();
        for (int i = 0; i < skillDataList.Count; i++)
        {
            randomIndices.Add(i);
        }


        for (int i = randomIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = randomIndices[i];
            randomIndices[i] = randomIndices[randomIndex];
            randomIndices[randomIndex] = temp;
        }

        for (int i = 0; i < total; i++)
        {
            var card = instances[i];
            if (card == null || targetPoints[i] == null)
            {
                completed++;
                continue;
            }

            int idx = randomIndices[i]; // 랜덤 인덱스 사용
            card.ApplyData(skillDataList[idx]);
            card.ResetCardAnim();
            card.gameObject.SetActive(true);

            // 애니메이션을 바로 시작 (이동 전에!)
            card.StartCardAnimation();

            var currentCard = card; // 클로저 문제 해결
            card.transform.DOMove(targetPoints[i].position, 0.3f)
                .OnComplete(() =>
                {
                    // floating 애니메이션 시작
                    currentCard.StartFloatingAnimation();
                    completed++;
                    if (completed >= total)
                    {
                        IsAnimating = false;
                        SetAllCanInteract(true);
                    }
                });
        }

        FadeImage(1f, 0f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                FadeImage(0f, 1f);
            });
        });
    }




    public void HideSkillCardList(int iAnimalNum = 0, Vector2 clickedCardPosition = default)
    {
        if (IsAnimating) return;
        IsAnimating = true;
        SetAllCanInteract(false);

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
                effectRect.DOAnchorPos(Vector2.zero, 0.8f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    FadeImage(1f, 0f).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(0.1f, () =>
                        {
                            FadeImage(0f, 1f);
                            IsAnimating = false;
                            DOVirtual.DelayedCall(1.2f, () =>
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




    public void SetAllCanInteract(bool canInteract)
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


    public SkillCard_SO FindSkillCardByName(string skillName)
    {
        return skillDataList.Find(card => card.sSkillName == skillName);
    }

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

        return Instantiate(skillPrefabs[actualIndex]);
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
}
