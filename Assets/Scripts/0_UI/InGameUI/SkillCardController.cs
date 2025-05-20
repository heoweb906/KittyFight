// SkillCardController.cs
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SkillCardController : MonoBehaviour
{
    public InGameUIController InGameUiController { get; set; }

    // �ִϸ��̼� �� ���� ���� üũ
    public bool IsAnimating { get; private set; }

    [Header("���ҽ� ����")]
    [Tooltip("Resources/SkillCards ���� ���")]
    [SerializeField] string skillCardResourceFolder = "SkillCards";

    [Header("��ų ī�� ������")]
    [SerializeField] GameObject objSkillCard;
    private List<SkillCard_SO> skillDataList = new List<SkillCard_SO>();
    private SkillCard_UI[] instances = new SkillCard_UI[4];

    [Header("ī�� ���� ��ġ �迭")]
    [SerializeField] Transform[] spawnPoints = new Transform[4];
    [SerializeField] Transform[] targetPoints = new Transform[4];
    

   

    /// <summary>
    /// ��ų ī�� ��Ʈ�ѷ� �ʱ�ȭ
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
                Debug.LogError($"[SkillCardController] Prefab �Ǵ� spawnPoints[{i}]�� �Ҵ���� �ʾҽ��ϴ�.");
                continue;
            }

            var go = Instantiate(objSkillCard, spawnPoints[i].position, Quaternion.identity, parent);
            var card = go.GetComponent<SkillCard_UI>();
            card.skillCardController = this;
            if (card == null)
            {
                Debug.LogError("[SkillCardController] objSkillCard Prefab�� SkillCard_UI ������Ʈ�� �����ϴ�.");
                continue;
            }

            card.ApplyData(null);
            card.gameObject.SetActive(false);
            instances[i] = card;
        }
    }

    /// <summary>
    /// ��� ��ų ī�带 Ȱ��ȭ�ϰ� �̵�
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
    /// ��� ��ų ī�带 ����� ���� ��ġ�� �̵�
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
