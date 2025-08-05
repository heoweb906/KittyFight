using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;

public class SkillCardController : MonoBehaviour 
{
    [Header("�߿��� ������")]
    public int iAuthorityPlayerNum = 0;
    public InGameUIController InGameUiController { get; set; }
    public bool IsAnimating { get; private set; }
   

    [Header("���ҽ� ����")]
    [Tooltip("Resources/SkillCards ���� ���")]
    [SerializeField] string skillCardResourceFolder = "SkillCards";


    [Header("��ų ������ ����Ʈ")]
    public List<GameObject> skillPrefabs;

    [Header("��ų ī�� ������")]
    [SerializeField] GameObject objSkillCard;
    public List<SkillCard_SO> skillDataList = new List<SkillCard_SO>();
    private SkillCard_UI[] instances = new SkillCard_UI[4];

    [Header("ī�� ���� ��ġ �迭")]
    [SerializeField] Transform[] spawnPoints = new Transform[4];
    [SerializeField] Transform[] targetPoints = new Transform[4];

    [Header("���⿡ ����� �͵�")]
    public Image image_FadeOut_White;
    public GameObject[] objs_AnimalSkillCardEffects;
    



    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        var datas = Resources.LoadAll<SkillCard_SO>(skillCardResourceFolder);

        // ���� ���� �Լ�
        int ExtractLeadingNumber(string name)
        {
            var match = Regex.Match(name, @"^\d+");
            if (match.Success)
                return int.Parse(match.Value);
            return int.MaxValue; // ���� ������ �� �ڷ�
        }

        var sortedDatas = datas.OrderBy(d => ExtractLeadingNumber(d.name)).ToArray();

        skillDataList.AddRange(sortedDatas);

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
            card.ApplyData(skillDataList[idx]);
            card.ResetCardAnim();


            card.gameObject.SetActive(true);
            var currentCard = card;
            card.transform.DOMove(targetPoints[i].position, 0.3f)
    .OnComplete(() =>
    {
        completed++;
        if (completed >= total)
        {
            IsAnimating = false;
            SetAllCanInteract(true);
        }
        currentCard.StartFloatingAnimation(); // ��鸲 ����
    });
        }
    }



    public void HideSkillCardList(int iAnimalNum = 0, Vector2 clickedCardPosition = default)
    {
        if (IsAnimating) return;
        IsAnimating = true;
        SetAllCanInteract(false);
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
            card.StopFloatingAnimation(); // ��鸲 ����
            card.transform.position = spawnPoints[i].position; // ��� �̵�
            card.gameObject.SetActive(false);
        }
        iAuthorityPlayerNum = 0;
        if (iAnimalNum >= 0 && iAnimalNum < objs_AnimalSkillCardEffects.Length)
        {
            GameObject effectObj = Instantiate(objs_AnimalSkillCardEffects[iAnimalNum], InGameUiController.canvasMain.transform);
            RectTransform effectRect = effectObj.GetComponent<RectTransform>();

            // image_FadeOut_White���� �ڿ� ���̰� ����
            effectRect.SetSiblingIndex(image_FadeOut_White.transform.GetSiblingIndex() - 1);

            // Ŭ���� ī�� ��ġ ���
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
                        });
                    });
                    Destroy(effectObj);
                });
            });
        }
        else
        {
            IsAnimating = false;
        }
    }





    public void SetAllCanInteract(bool canInteract)
    {
        Debug.Log("���� " + canInteract + "�� ������");

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

        int idx = card.iSkillIndex;
        if (idx < 0 || idx >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillCardController] skillIndex({idx}) is out of range.");
            return null;
        }

        return Instantiate(skillPrefabs[idx]);
    }




    public Tween FadeImage(float fTargetAlpha, float duration)
    {
        // ������ ��, ��ǥ ���İ��� 0�� �ƴ϶�� Ȱ��ȭ
        if (fTargetAlpha != 0f)
        {
            image_FadeOut_White.gameObject.SetActive(true);
        }

        image_FadeOut_White.transform.SetAsLastSibling();
        return image_FadeOut_White.DOFade(fTargetAlpha, duration)
            .OnComplete(() =>
            {
                // ������ ���İ��� 0�̶�� ��Ȱ��ȭ
                if (fTargetAlpha == 0f)
                {
                    image_FadeOut_White.gameObject.SetActive(false);
                }
            });
    }
}
