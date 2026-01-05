using System.Collections;
using UnityEngine;

public class MapGimic_5_Dragon : AbstractMapGimic
{
    // 붉은색 계열 (Dragon Color)
    private readonly Color dragonColor = new Color(1f, 0.2f, 0.2f);

    [Header("Spawn Settings")]
    public GameObject laserPrefab; // 기본: 왼쪽 -> 오른쪽으로 나가는 프리팹

    [Header("Map Boundaries")]
    public float area_xMin;
    public float area_xMax;
    public float area_yMin;
    public float area_yMax;
    public float spawnZ = 0f; // 레이저 높이

    [SerializeField] private float spawnInterval = 2.0f; // 생성 주기

    private Coroutine spawnCoroutine;

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(dragonColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(dragonColor));
                StartCoroutine(Co_DragonSequence());
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        if (MatchResultStore.myPlayerNumber == 1)
        {
            if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
        }
    }

    private IEnumerator Co_DragonSequence()
    {
        SendTween(0.2f, 0.4f);
        yield return new WaitForSeconds(0.25f);

        // 스폰 시작 (호스트만)
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(Co_SpawnRoutine());

        SendTween(1f, 0.5f);
    }

    // [Host] 랜덤 벽 선택, 위치 및 회전 계산 후 전송
    private IEnumerator Co_SpawnRoutine()
    {
        float margin = 1.0f; // 모서리 여유 간격

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 0:좌(→), 1:우(←), 2:하(↑), 3:상(↓)
            int sideIndex = Random.Range(0, 4);

            Vector3 spawnPos = Vector3.zero;
            float rotationZ = 0f; // 2D 회전 각도

            switch (sideIndex)
            {
                case 0: // 왼쪽 벽에서 오른쪽으로 발사
                    // Y축 랜덤 범위 축소 (위아래 여유)
                    spawnPos = new Vector3(area_xMin, Random.Range(area_yMin + margin, area_yMax - margin), spawnZ);
                    rotationZ = 0f;
                    break;

                case 1: // 오른쪽 벽에서 왼쪽으로 발사
                    // Y축 랜덤 범위 축소 (위아래 여유)
                    spawnPos = new Vector3(area_xMax, Random.Range(area_yMin + margin, area_yMax - margin), spawnZ);
                    rotationZ = 180f;
                    break;

                case 2: // 아래쪽 벽에서 위로 발사
                    // X축 랜덤 범위 축소 (양옆 여유)
                    spawnPos = new Vector3(Random.Range(area_xMin + margin, area_xMax - margin), area_yMin, spawnZ);
                    rotationZ = 90f;
                    break;

                case 3: // 위쪽 벽에서 아래로 발사
                    // X축 랜덤 범위 축소 (양옆 여유)
                    spawnPos = new Vector3(Random.Range(area_xMin + margin, area_xMax - margin), area_yMax, spawnZ);
                    rotationZ = -90f;
                    break;
            }

            // 1. 내 화면 생성
            SpawnEntity(spawnPos, rotationZ);

            // 2. 패킷 전송
            P2PMessageSender.SendMessage(
                MapGimicBuilder.BuildDragon_Spawn(mapManager.GetMapGimicIndex(), spawnPos.x, spawnPos.y, spawnPos.z, rotationZ)
            );
        }
    }

    // [공통] 실제 생성 로직
    private void SpawnEntity(Vector3 pos, float angleZ)
    {
        if (laserPrefab != null)
        {
            // Z축 기준으로 회전 생성 (2D 게임 기준)
            Quaternion rot = Quaternion.Euler(0, 0, angleZ);

            var obj = Instantiate(laserPrefab, pos, rot);

            var entity = obj.GetComponent<Gimic5_Entity>();
            if (entity != null)
            {
                // 여기서 mapManager.gameManager 또는 FindObjectOfType을 사용합니다.
                // 기믹 매니저에 이미 참조가 있다면 그것을 쓰고, 없다면 아래처럼 찾아서 넣어줍니다.
                entity.manager = FindObjectOfType<GameManager>();
            }

            // 필요 시 히트박스 중립 설정 추가
            // var hitbox = obj.GetComponent<AB_HitboxBase>();
            // if (hitbox) hitbox.bMiddleState = true; 
        }
    }

    // [Client] 패킷 수신
    public void ReceiveSpawnSync(float x, float y, float z, float angleZ)
    {
        SpawnEntity(new Vector3(x, y, z), angleZ);
    }
}