using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGimic_8_Sheep : AbstractMapGimic
{
    // 초록색 (Green)
    private readonly Color sheepColor = new Color(0.2f, 1f, 0.2f);

    [Header("Spawn Settings")]
    public GameObject sheepObj; // Gimic8_Entity Prefab
    public Transform point_LeftBottom;
    public Transform point_RightUp;

    [Header("Distribution Settings")]
    [SerializeField] private float minDist = 4.0f; // 양들끼리의 최소 거리
    [SerializeField] private int maxSpawnAttempts = 20; // 위치 찾기 시도 횟수

    // 생성된 오브젝트들을 관리하기 위한 리스트
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(sheepColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(sheepColor));

                // 시작 시 3개 생성
                SpawnSheeps(3);
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        // [수정] 즉시 삭제하지 않고 코루틴 실행
        StartCoroutine(Co_DelayedCleanup());

        if (MatchResultStore.myPlayerNumber == 1)
        {
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
        }
    }

    // [추가] 2초 대기 후 삭제하는 코루틴
    private IEnumerator Co_DelayedCleanup()
    {
        yield return new WaitForSeconds(2.0f);
        CleanupEntities();
    }

    // [Host] 랜덤 위치 + 벽 체크 + 거리 체크
    private void SpawnSheeps(int count)
    {
        if (point_LeftBottom == null || point_RightUp == null) return;

        List<Vector3> currentSpawnPositions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                float randX = Random.Range(point_LeftBottom.position.x, point_RightUp.position.x);
                float randY = Random.Range(point_LeftBottom.position.y, point_RightUp.position.y);
                float fixedZ = point_LeftBottom.position.z;

                Vector3 candidatePos = new Vector3(randX, randY, fixedZ);

                if (CheckWallOverlap(candidatePos)) continue;

                bool isTooClose = false;
                foreach (var pos in currentSpawnPositions)
                {
                    if (Vector3.Distance(candidatePos, pos) < minDist)
                    {
                        isTooClose = true;
                        break;
                    }
                }
                if (isTooClose) continue;

                currentSpawnPositions.Add(candidatePos);

                SpawnEntity(candidatePos.x, candidatePos.y, candidatePos.z);

                P2PMessageSender.SendMessage(
                    MapGimicBuilder.BuildSheep_Spawn(mapManager.GetMapGimicIndex(), candidatePos.x, candidatePos.y, candidatePos.z)
                );

                break;
            }
        }
    }

    private bool CheckWallOverlap(Vector3 centerPos)
    {
        Vector3 halfExtents = new Vector3(0.45f, 0.45f, 0.45f);
        Collider[] hitColliders = Physics.OverlapBox(centerPos, halfExtents, Quaternion.identity);

        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Wall")) return true;
        }
        return false;
    }

    private void SpawnEntity(float x, float y, float z)
    {
        if (sheepObj != null)
        {
            Vector3 spawnPos = new Vector3(x, y, z);
            GameObject obj = Instantiate(sheepObj, spawnPos, Quaternion.identity);

            spawnedObjects.Add(obj);
        }
    }

    private void CleanupEntities()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    public void ReceiveSpawnSync(float x, float y, float z)
    {
        SpawnEntity(x, y, z);
    }
}