using System.Collections;
using UnityEngine;

public class MapGimic_10_Chicken : AbstractMapGimic
{
    private readonly Color chickenColor = new Color(1f, 0.92f, 0.6f);

    [Header("Spawn Settings")]
    public GameObject createEntity;
    public Transform point_Left;  // 왼쪽 끝 기준점
    public Transform point_Right; // 오른쪽 끝 기준점

    [SerializeField] private float fallingSpeed = 5.0f; // 떨어지는 초기 속도 (필요시)

    private Coroutine spawnCoroutine;

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(chickenColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(chickenColor));
                StartCoroutine(Co_ChickenSequence());
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

    private IEnumerator Co_ChickenSequence()
    {
        SendTween(0.2f, 0.4f);
        yield return new WaitForSeconds(0.25f);

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(Co_SpawnRoutine());

        SendTween(1f, 0.5f);
    }

    // [Host] 랜덤 값 생성 및 전송
    private IEnumerator Co_SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);

            if (point_Left != null && point_Right != null)
            {
                // 1. 랜덤 위치 비율 (0.0 ~ 1.0)
                float randomRatio = Random.value;

                // 2. 랜덤 회전 각도 (0 ~ 360)
                float randomRotZ = Random.Range(0f, 360f);

                // 3. 내 화면 생성
                SpawnEntity(randomRatio, randomRotZ);

                // 4. 패킷 전송
                P2PMessageSender.SendMessage(
                    MapGimicBuilder.BuildChicken_Spawn(mapManager.GetMapGimicIndex(), randomRatio, randomRotZ)
                );
            }
        }
    }


    // [공통] 실제 생성 로직 (위치 보간 + 회전 적용 + 물리 설정)
    private void SpawnEntity(float ratio, float rotZ)
    {
        if (createEntity == null || point_Left == null || point_Right == null) return;

        // 1. 위치 계산
        Vector3 spawnPos = Vector3.Lerp(point_Left.position, point_Right.position, ratio);

        // 2. 초기 회전값 적용
        Quaternion rot = Quaternion.Euler(0, 0, rotZ);

        // 3. 생성
        var obj = Instantiate(createEntity, spawnPos, rot);

        // 4. 물리 설정 (강한 회전 적용)
        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            rb.maxAngularVelocity = 100f; // 회전 속도 제한 해제
            rb.velocity = Vector3.down * fallingSpeed; // 초기 낙하 속도

            // 강한 랜덤 회전
            float randomSpin = Random.Range(-50f, 50f);
            rb.angularVelocity = new Vector3(0, 0, randomSpin);
        }

        // 5. [추가] 중립 상태(MiddleState) 설정
        // 생성된 객체(또는 자식)에 AB_HitboxBase가 있다면 bMiddleState를 true로 켜줌
        var hitbox = obj.GetComponent<AB_HitboxBase>();
        if (hitbox != null)
        {
            hitbox.Init(gameManager.playerAbility_1);
            hitbox.bMiddleState = true;
            // 중립 오브젝트는 소유자(ownerAbility)가 없어도 충돌 판정이 가능해짐
        }
    }

    // ... (이후 코드 동일)

    // [Client] 패킷 수신
    public void ReceiveSpawnSync(float ratio, float rotZ)
    {
        SpawnEntity(ratio, rotZ);
    }
}