// SkillCardController.cs
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SkillCardController : MonoBehaviour
{
    public InGameUIController InGameUiController { get; set; }

    // 애니메이션 및 선택 상태 체크
    public bool IsAnimating { get; private set; }

    [Header("리소스 설정")]
    [Tooltip("Resources/SkillCards 폴더 경로")]
    [SerializeField] string skillCardResourceFolder = "SkillCards";

    [Header("스킬 카드 프리팹")]
    [SerializeField] GameObject objSkillCard;
    private List<SkillCard_SO> skillDataList = new List<SkillCard_SO>();
    private SkillCard_UI[] instances = new SkillCard_UI[4];

    [Header("카드 생성 위치 배열")]
    [SerializeField] Transform[] spawnPoints = new Transform[4];
    [SerializeField] Transform[] targetPoints = new Transform[4];
    

   

    /// <summary>
    /// 스킬 카드 컨트롤러 초기화
    /// </summary>
    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;


        var datas = Resources.LoadAll<SkillCard_SO>(skillCardResourceFolder);
        skillDataList.AddRange(datas);

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

    /// <summary>
    /// 모든 스킬 카드를 활성화하고 이동
    /// </summary>
    public void ShowAll()
    {
        if (skillDataList.Count == 0 || IsAnimating) return;
        IsAnimating = true;


        int completed = 0;
        int total = instances.Length;

        for (int i = 0; i < total; i++)
        {
            var card = instances[i];
            if (card == null || targetPoints[i] == null)
            {
                completed++;
                continue;
            }

            int idx = Random.Range(0, skillDataList.Count);
            card.ApplyData(skillDataList[idx]);
            card.gameObject.SetActive(true);
            card.transform.DOMove(targetPoints[i].position, 0.3f)
                .OnComplete(() =>
                {
                    completed++;
                    if (completed >= total)
                        IsAnimating = false;
                        SetAllCanInteract(true);

                });
        }
    }

    /// <summary>
    /// 모든 스킬 카드를 숨기고 생성 위치로 이동
    /// </summary>
    public void HideAll()
    {
        if (IsAnimating) return;
        IsAnimating = true;

        SetAllCanInteract(false);

        int completed = 0;
        int total = instances.Length;

        for (int i = 0; i < total; i++)
        {
            var card = instances[i];
            if (card == null || spawnPoints[i] == null)
            {
                completed++;
                continue;
            }

            card.transform.DOMove(spawnPoints[i].position, 0.2f)
                .OnComplete(() =>
                {
                    card.gameObject.SetActive(false);
                    completed++;
                    if (completed >= total)
                        IsAnimating = false;
 
                });
        }
    }



    public void SetAllCanInteract(bool canInteract)
    {
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
}
