// SkillCardController.cs
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SkillCardController : MonoBehaviour 
{
    [Header("중요한 정보들")]
    public int iAuthorityPlayerNum = 0;

    public InGameUIController InGameUiController { get; set; }
    // 애니메이션 및 선택 상태 체크
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



    public void ShowSkillCardList(int iPlayernum = 0)
    {
        if (skillDataList.Count == 0 || IsAnimating) return;
        iAuthorityPlayerNum = iPlayernum;


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

    public void HideSkillCardList()
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

        iAuthorityPlayerNum = 0;
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

        int idx = card.skillIndex;
        if (idx < 0 || idx >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillCardController] skillIndex({idx}) is out of range.");
            return null;
        }

        return Instantiate(skillPrefabs[idx]);
    }
}
