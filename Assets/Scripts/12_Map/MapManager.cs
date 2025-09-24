using UnityEngine;

public class MapManager : MonoBehaviour
{
    // [Header("�� ������")]
    // public GameObject[] mapPrefabs; // -> �� �κ��� �Ʒ� �ڵ�� ����

    [Header("���� ��ġ�� �� ������Ʈ")]
    public GameObject[] mapObjects; // �ν����Ϳ��� ���� �ʵ��� ����� �巡��

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

    // ���� �Ѱ� ���� ������� ���� ����
    public void ChangeMap(int mapIndex)
    {
        // ��ȿ�� �ε������� Ȯ��
        if (mapIndex < 0 || mapIndex >= mapObjects.Length)
        {
            Debug.LogError($"Invalid map index: {mapIndex}");
            return;
        }

        // ��� ���� ��ȸ�ϸ鼭
        for (int i = 0; i < mapObjects.Length; i++)
        {
            if (mapObjects[i] == null) continue;

            // ���õ� �ε����� �ʸ� Ȱ��ȭ�ϰ�, �������� ��� ��Ȱ��ȭ�մϴ�.
            bool shouldBeActive = (i == mapIndex);
            mapObjects[i].SetActive(shouldBeActive);

            // Ȱ��ȭ�� ���̶��, MapLayout ������ �����ɴϴ�.
            if (shouldBeActive)
            {
                currentMapLayout = mapObjects[i].GetComponent<MapLayout>();
            }
        }

        if (currentMapLayout == null)
        {
            Debug.LogError($"Map object '{mapObjects[mapIndex].name}' is missing MapLayout component!");
        }
    }

    public Transform GetSpawnPoint(int playerNumber)
    {
        if (currentMapLayout == null)
        {
            Debug.LogError("Current Map Layout is not set!");
            return this.transform; // �ӽ÷� �ڱ� �ڽ��� ��ġ�� ��ȯ
        }
        return playerNumber == 1 ? currentMapLayout.spawnPoint1 : currentMapLayout.spawnPoint2;
    }
}