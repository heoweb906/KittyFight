using UnityEngine;

public class MapManager : MonoBehaviour
{
    // [Header("맵 프리팹")]
    // public GameObject[] mapPrefabs; // -> 이 부분을 아래 코드로 변경

    [Header("씬에 배치된 맵 오브젝트")]
    public GameObject[] mapObjects; // 인스펙터에서 씬의 맵들을 여기로 드래그

    private MapLayout currentMapLayout;

    void Awake()
    {
        // 게임 시작 시 모든 맵을 일단 꺼둡니다.
        foreach (var map in mapObjects)
        {
            if (map != null)
            {
                map.SetActive(false);
            }
        }
    }

    // 맵을 켜고 끄는 방식으로 로직 변경
    public void ChangeMap(int mapIndex)
    {
        // 유효한 인덱스인지 확인
        if (mapIndex < 0 || mapIndex >= mapObjects.Length)
        {
            Debug.LogError($"Invalid map index: {mapIndex}");
            return;
        }

        // 모든 맵을 순회하면서
        for (int i = 0; i < mapObjects.Length; i++)
        {
            if (mapObjects[i] == null) continue;

            // 선택된 인덱스의 맵만 활성화하고, 나머지는 모두 비활성화합니다.
            bool shouldBeActive = (i == mapIndex);
            mapObjects[i].SetActive(shouldBeActive);

            // 활성화된 맵이라면, MapLayout 정보를 가져옵니다.
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
            return this.transform; // 임시로 자기 자신의 위치를 반환
        }
        return playerNumber == 1 ? currentMapLayout.spawnPoint1 : currentMapLayout.spawnPoint2;
    }
}