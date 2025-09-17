using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;



public class SkillCardController_Test : MonoBehaviour
{
    [Header("�߿��� ������")]
    public int iAuthorityPlayerNum = 0;
    public Canvas targetCanvas;
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
    public GameObject[] objs_AnimalSkillCardEffects;
    public Image image_FadeOut_White;

    [Header("�׽�Ʈ��")]
    public KeyCode keyShowCards = KeyCode.Space;
    public KeyCode keyHideCards = KeyCode.Escape;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ShowSkillCardList(1, true);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            HideSkillCardList(0);
        }
    }

    private void Start()
    {
        if (targetCanvas == null)
            targetCanvas = GetComponentInParent<Canvas>();

        Initialize(targetCanvas, transform);
    }

    public void Initialize(Canvas canvas, Transform parent)
    {
        targetCanvas = canvas;

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

        // 0�� ������ �����ϰ� �߰�
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
                Debug.LogError($"[SkillCardController_Test] Prefab �Ǵ� spawnPoints[{i}]�� �Ҵ���� �ʾҽ��ϴ�.");
                continue;
            }

            var go = Instantiate(objSkillCard, spawnPoints[i].position, Quaternion.identity, parent);
            var card = go.GetComponent<SkillCard_UI>();

            // card.skillCardController = this;

            if (card == null)
            {
                Debug.LogError("[SkillCardController_Test] objSkillCard Prefab�� SkillCard_UI ������Ʈ�� �����ϴ�.");
                continue;
            }

            card.ApplyData(null);
            card.gameObject.SetActive(false);
            instances[i] = card;
        }
    }

    // #. ��ų �����ִ� �Լ�
    public void ShowSkillCardList(int iPlayernum = 0, bool bActivePassive = true, int[] iCardArray = null)
    {
        if (skillDataList.Count == 0 || IsAnimating) return;
        iAuthorityPlayerNum = iPlayernum;
        IsAnimating = true;

        List<int> selectedIndices = new List<int>();

        // iCardArray�� ������ �� ������ ���
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
            // ���ǿ� �´� ��ų�� ���͸�
            List<int> filteredIndices = new List<int>();
            for (int i = 0; i < skillDataList.Count; i++)
            {
                bool isActive = skillDataList[i].iSkillIndex < 100;
                if ((bActivePassive && isActive) || (!bActivePassive && !isActive))
                {
                    filteredIndices.Add(i);
                }
            }

            // ��� ������ ��ų�� ������ ����
            if (filteredIndices.Count == 0)
            {
                IsAnimating = false;
                return;
            }

            List<int> availableIndices = new List<int>(filteredIndices);

            // �ߺ� ���� ����
            for (int i = 0; i < instances.Length && availableIndices.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, availableIndices.Count);
                int selectedIdx = availableIndices[randomIndex];
                selectedIndices.Add(selectedIdx);
                availableIndices.RemoveAt(randomIndex); // ���õ� ���� ����
            }

            StartShowingCards(selectedIndices);
        }
    }

    // ���� ī�� ǥ�� ������ �и��� �Լ�
    private void StartShowingCards(List<int> selectedIndices)
    {
        int completed = 0;
        int total = instances.Length;
        bool hasError = false;

        FadeImage(1f, 0f).OnComplete(() =>
        {
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
                                // Tween�� �ߴܵ� ��쿡�� completed ����
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
                        Debug.LogError($"[SkillCardController_Test] Error showing card {i}: {e.Message}");
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

    // ī�� ǥ�� �Ϸ� ó���� ���� �Լ��� �и�
    private void CompleteCardShow()
    {
        IsAnimating = false;
        SetAllCanInteract(true);
    }

    public void HideSkillCardList(int iAnimalNum = 0, Vector2 clickedCardPosition = default)
    {
        if (IsAnimating) return;
        IsAnimating = true;
        SetAllCanInteract(false);

        // floating �ִϸ��̼� ����
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
            card.transform.position = spawnPoints[i].position; // ��� �̵�
            card.gameObject.SetActive(false);
        }

        iAuthorityPlayerNum = 0;

        if (iAnimalNum >= 0 && iAnimalNum < objs_AnimalSkillCardEffects.Length)
        {
            GameObject effectObj = Instantiate(objs_AnimalSkillCardEffects[iAnimalNum], targetCanvas.transform);
            RectTransform effectRect = effectObj.GetComponent<RectTransform>();

            // image_FadeOut_White���� �ڿ� ���̰� ����
            effectRect.SetSiblingIndex(image_FadeOut_White.transform.GetSiblingIndex() - 1);

            // Ŭ���� ī�� ��ġ ���
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
            Debug.LogWarning("[SkillCardController_Test] SkillCard_SO is null.");
            return null;
        }

        // skillDataList���� �ش� ī���� ���� �ε��� ã��
        int actualIndex = skillDataList.IndexOf(card);
        if (actualIndex < 0)
        {
            Debug.LogError($"[SkillCardController_Test] Card '{card.name}' not found in skillDataList.");
            return null;
        }

        if (actualIndex >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillCardController_Test] actualIndex({actualIndex}) is out of range. skillPrefabs.Count = {skillPrefabs.Count}");
            return null;
        }

        GameObject skillObj = Instantiate(skillPrefabs[actualIndex]);
        Skill skill = skillObj.GetComponent<Skill>();
        if (skill != null)
        {
            skill.SkillIndex = actualIndex;  // �ε��� ����
        }

        return skillObj;
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
