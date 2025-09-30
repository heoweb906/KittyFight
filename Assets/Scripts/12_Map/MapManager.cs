using UnityEngine;

/// <summary>
/// �ʰ� ����� �ε� �� ������ ��� �����ϴ� ���� Ŭ�����Դϴ�.
/// </summary>
public class MapManager : MonoBehaviour
{
    [Header("�� ����")]
    [Tooltip("Hierarchy ���� �� �θ� ������Ʈ���� ������� ������ּ���.")]
    public GameObject[] mapObjects;

    [Header("��� ����")]
    [Tooltip("����� ǥ���� SpriteRenderer ������Ʈ�� �Ҵ��ϼ���.")]
    public SpriteRenderer backgroundSpriteRenderer;
    [Tooltip("����� ��� ��������Ʈ ���")]
    public Sprite[] backgroundSprites;

    private MapLayout currentMapLayout;

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
}