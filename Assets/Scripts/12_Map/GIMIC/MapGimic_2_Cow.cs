using System.Collections;
using UnityEngine;

public class MapGimic_2_Cow : AbstractMapGimic
{
    [Header("Gimmick Settings")]
    [SerializeField] private float activationInterval = 12.0f;

    [Header("Earthquake Settings")]
    [SerializeField] private GameObject objSkillEntity;
    [SerializeField] private float shakeAmount = 2f;
    [SerializeField] private float shakeDuration = 0.5f;

    private float timer;
    private bool isSequenceStarted = false;

    private readonly Color vermilionColor = new Color(1f, 0.5f, 0f);

    public override void OnGimicStart()
    {
        base.OnGimicStart();
        timer = 0f;
        isSequenceStarted = false;

        if (mapManager != null)
        {
            mapManager.SetScreenColor(vermilionColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(vermilionColor));
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        if (MatchResultStore.myPlayerNumber == 1)
        {
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
        }
    }

    public override void OnGimmickUpdate()
    {
        if (MatchResultStore.myPlayerNumber == 1)
        {
            timer += Time.deltaTime;

            if (!isSequenceStarted && timer >= (activationInterval - 8.0f))
            {
                isSequenceStarted = true;
                StartCoroutine(Co_PlayCowSequence());
            }

            if (timer >= activationInterval)
            {
                timer = 0f;
                isSequenceStarted = false;
            }
        }
    }
   

    private IEnumerator Co_PlayCowSequence()
    {
        SendTween(1f, 0.44f);
        yield return new WaitForSeconds(1f);

        SendTween(1f, 0.5f);
        yield return new WaitForSeconds(1f);

        SendTween(1f, 0.44f);
        yield return new WaitForSeconds(1f);

        SendTween(1f, 0.5f);
        yield return new WaitForSeconds(1f);

        SendTween(0.15f, 0.4f);

        // [수정됨] 호스트 실행 및 패킷 전송
        ExecuteEarthquake_Host();

        yield return new WaitForSeconds(0.15f);

        SendTween(0.15f, 0.5f);
    }

    // 1. 실제 오브젝트 생성 및 카메라 연출 (공통 로직)
    private void SpawnEarthquakeShared()
    {
        // 로컬 클라이언트 기준의 PlayerAbility를 찾음
        var targetPlayer = FindObjectOfType<PlayerAbility>();

        if (targetPlayer != null && objSkillEntity != null)
        {
            GameObject hitbox = Instantiate(objSkillEntity, targetPlayer.transform.position, Quaternion.identity);
            var ab = hitbox.GetComponent<AB_HitboxBase>();
            if (ab != null) ab.Init(targetPlayer);

            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
        }
    }
    
    public void ReceiveCowSync(int actionType)
    {
        // 여기서 로직 분기 (1: 지진)
        if (actionType == 1)
        {
            SpawnEarthquakeShared(); // 호스트 명령으로 생성();
        }
    }


    // 2. 호스트 전용: 로컬 실행 + 리스너에게 패킷 전송
    private void ExecuteEarthquake_Host()
    {
        SpawnEarthquakeShared();

        // [수정] Packet_2_Cow의 actionType=1 (지진) 패킷 생성 호출
        P2PMessageSender.SendMessage(MapGimicBuilder.Build_Cow_Earthquake());
    }


}