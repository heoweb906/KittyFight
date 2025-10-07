using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ʰ� ����� �ε� �� ������ ��� �����ϴ� ���� Ŭ�����Դϴ�.
/// </summary>
public class MapManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("�� ����")]
    [Tooltip("Hierarchy ���� �� �θ� ������Ʈ���� ������� ������ּ���.")]
    public GameObject[] mapObjects;

    [Header("��� ����")]
    [Tooltip("����� ǥ���� SpriteRenderer ������Ʈ�� �Ҵ��ϼ���.")]
    public SpriteRenderer backgroundSpriteRenderer;
    [Tooltip("����� ��� ��������Ʈ ���")]
    public Sprite[] backgroundSprites;

    private MapLayout currentMapLayout;


    [Header("�� ��� ����")]
    // �����ʿ�
    public Image iamge_TestMapGimicColor;

    [SerializeField] private List<AbstractMapGimic> gimicks;
    [HideInInspector] private AbstractMapGimic currentGimmick;

    public int currentIndex = -1;



    void Awake()
    {
        // ���� ���� �� ��� ���� �ϴ� ���Ӵϴ�.
        foreach (var map in mapObjects)
        {
            if (map != null)
            {
                map.SetActive(false);
            }
        }
    }


    private void Start()
    {
        for (int i = 0; i < gimicks.Count; ++i) gimicks[i].gameManger = gameManager;
        for (int i = 0; i < gimicks.Count; ++i) gimicks[i].mapManager = this;

        currentGimmick = null;
    }



    private void FixedUpdate()
    {
        if (currentGimmick != null)
            currentGimmick.OnGimmickUpdate();
    }




    /// <summary>
    /// ������ �ε����� ���� Ȱ��ȭ�ϰ� ������ �ʵ��� ��� ��Ȱ��ȭ�մϴ�.
    /// </summary>
    public void ChangeMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= mapObjects.Length) return;
      

        for (int i = 0; i < mapObjects.Length; i++)
        {
            if (mapObjects[i] == null) continue;
            bool shouldBeActive = (i == mapIndex);
            mapObjects[i].SetActive(shouldBeActive);

            if (shouldBeActive)
            {
                currentMapLayout = mapObjects[i].GetComponent<MapLayout>();
            }
        }
    }

    /// <summary>
    /// ������ �ε����� ��������Ʈ�� ����� �����մϴ�.
    /// </summary>
    public void ChangeBackground(int bgIndex)
    {
        if (bgIndex < 0 || bgIndex >= backgroundSprites.Length) return;
        if (backgroundSpriteRenderer != null)
        {
            backgroundSpriteRenderer.sprite = backgroundSprites[bgIndex];
        }
    }

    /// <summary>
    /// ���� Ȱ��ȭ�� ���� ���� ������ ��ȯ�մϴ�.
    /// </summary>
    public Transform GetSpawnPoint(int playerNumber)
    {
        if (currentMapLayout == null)
        {
            return this.transform;
        }
        return playerNumber == 1 ? currentMapLayout.spawnPoint1 : currentMapLayout.spawnPoint2;
    }




    #region // �� ��� ��Ʈ�� ����


    public void ChangeMapGimicIndex(int iGimicIndex)
    {
        currentIndex = iGimicIndex - 1;
    }


    public void ActivateGimmick()
    {
        if (currentGimmick != null) currentGimmick.OnGimicEnd();

        if (currentIndex < 0 || currentIndex >= gimicks.Count)
        {
            currentGimmick = null;
            currentIndex = -1;
            return;
        }

        currentGimmick = gimicks[currentIndex];
        currentGimmick.OnGimicStart();
    }



    public void StopCurrentGimmick()
    {
        if (currentGimmick != null)
        {
            currentGimmick.OnGimicEnd();
            currentGimmick = null;
            currentIndex = -1;
        }
    }
    #endregion
}
