using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGimic_12_Pig : AbstractMapGimic
{
    // 핑크색 (Pink)
    private readonly Color pigColor = new Color(1f, 0.4f, 0.7f);

    [Header("Spawn Settings")]
    public GameObject warningPrefab; // 돼지(아이템) 생성용 프리팹
    public Transform point_LeftBottom;
    public Transform point_RightUp;

    [SerializeField] private float spawnInterval = 3.0f; // 생성 주기
    [SerializeField] private int maxSpawnAttempts = 10;  // 벽 피하기 시도 횟수

    private Coroutine spawnCoroutine;

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(pigColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(pigColor));
                StartCoroutine(Co_PigSequence());
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        // 1. 스폰 코루틴 정지
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);

        // 2. [추가] 2초 뒤에 남아있는 아이템(Gimic12_Entity) 삭제
        StartCoroutine(Co_CleanupEntities());

        if (MatchResultStore.myPlayerNumber == 1)
        {
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
        }
    }

    private IEnumerator Co_PigSequence()
    {
        // 1. 연출
        SendTween(0.2f, 0.4f);
        yield return new WaitForSeconds(0.25f);

        // 2. 스폰 시작 (호스트만)
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(Co_SpawnRoutine());

        // 3. 복귀
        SendTween(1f, 0.5f);
    }

    // [Host] 랜덤 위치 계산 (벽 피하기 로직 포함) 및 전송
    private IEnumerator Co_SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (point_LeftBottom != null && point_RightUp != null)
            {
                Vector3 spawnPos = Vector3.zero;
                bool validPosFound = false;

                // 벽이 없는 위치를 찾을 때까지 시도
                for (int i = 0; i < maxSpawnAttempts; i++)
                {
                    float randX = Random.Range(point_LeftBottom.position.x, point_RightUp.position.x);
                    float randY = Random.Range(point_LeftBottom.position.y, point_RightUp.position.y);
                    float fixedZ = point_LeftBottom.position.z;

                    Vector3 tempPos = new Vector3(randX, randY, fixedZ);

                    // [추가] 1x1 범위(반지름 0.5) 내에 Wall 태그가 있는지 검사
                    if (!CheckWallOverlap(tempPos))
                    {
                        spawnPos = tempPos;
                        validPosFound = true;
                        break; // 안전한 위치 찾음
                    }
                }

                // 유효한 위치를 찾았을 때만 생성 및 전송
                if (validPosFound)
                {
                    // 1. 내 화면 생성
                    SpawnEntity(spawnPos.x, spawnPos.y, spawnPos.z);

                    // 2. 패킷 전송
                    P2PMessageSender.SendMessage(
                        MapGimicBuilder.BuildPig_Spawn(mapManager.GetMapGimicIndex(), spawnPos.x, spawnPos.y, spawnPos.z)
                    );
                }
            }
        }
    }

    // Wall 태그와 겹치는지 확인하는 함수
    private bool CheckWallOverlap(Vector3 centerPos)
    {
        // 1x1 박스니까 HalfExtents는 0.5f (조금 여유있게 0.4로 되어있음)
        Vector3 halfExtents = new Vector3(0.4f, 0.4f, 0.4f);

        // 해당 영역의 모든 콜라이더 검출
        Collider[] hitColliders = Physics.OverlapBox(centerPos, halfExtents, Quaternion.identity);

        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Wall"))
            {
                return true; // 벽이 있음
            }
        }
        return false; // 벽이 없음
    }

    private void SpawnEntity(float x, float y, float z)
    {
        if (warningPrefab != null)
        {
            Vector3 spawnPos = new Vector3(x, y, z);
            // [수정] X축 -90도 회전 적용
            Instantiate(warningPrefab, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
        }
    }

    // [추가] 종료 시 정리 코루틴
    private IEnumerator Co_CleanupEntities()
    {
        // 2초 대기
        yield return new WaitForSeconds(2.0f);

        // 씬에 있는 모든 Gimic12_Entity를 찾아서 삭제 (돼지 아이템 스크립트 이름 가정)
        Gimic12_Entity[] entities = FindObjectsOfType<Gimic12_Entity>();
        foreach (var entity in entities)
        {
            if (entity != null)
            {
                Destroy(entity.gameObject);
            }
        }
    }

    // [Client] 패킷 수신
    public void ReceiveSpawnSync(float x, float y, float z)
    {
        SpawnEntity(x, y, z);
    }
}