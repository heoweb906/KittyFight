using System.Collections;
using UnityEngine;

public class MapGimic_9_Monkey : AbstractMapGimic
{
    [Header("Gimmick Settings")]
    [SerializeField] private float activationInterval = 12.0f;

    private float timer;
    private bool isSequenceStarted = false;

    // 하늘색 (Sky Blue)
    private readonly Color skyBlueColor = new Color(0.53f, 0.81f, 0.98f);

    public override void OnGimicStart()
    {
        base.OnGimicStart();
        timer = 0f;
        isSequenceStarted = false;

        if (mapManager != null)
        {
            mapManager.SetScreenColor(skyBlueColor);
            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(skyBlueColor));
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
        // 호스트(P1)만 타이머 체크 및 연출 시작
        if (MatchResultStore.myPlayerNumber == 1)
        {
            timer += Time.deltaTime;

            if (!isSequenceStarted && timer >= (activationInterval - 8.0f))
            {
                isSequenceStarted = true;
                StartCoroutine(Co_PlayMonkeySequence());
            }

            if (timer >= activationInterval)
            {
                timer = 0f;
                isSequenceStarted = false;
            }
        }
    }

    private IEnumerator Co_PlayMonkeySequence()
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

        // 로직 실행 (쿨타임 초기화)
        ExecuteMonkeyLogic_Host();

        yield return new WaitForSeconds(0.15f);
        SendTween(0.15f, 0.5f);
    }

    // [공통] 양측 플레이어 쿨타임 초기화
    private void ResetCooldownsShared()
    {
        if (gameManager == null) return;

        // Player 1 쿨타임 초기화
        if (gameManager.playerAbility_1 != null)
        {
            gameManager.playerAbility_1.ResetAllCooldowns();
        }

        // Player 2 쿨타임 초기화
        if (gameManager.playerAbility_2 != null)
        {
            gameManager.playerAbility_2.ResetAllCooldowns();
        }

        Debug.Log("[Monkey] 양측 플레이어 스킬 쿨타임 초기화 완료");
    }

    // [Host] 실행 및 패킷 전송
    private void ExecuteMonkeyLogic_Host()
    {
        ResetCooldownsShared();

        // 리스너에게 실행 명령 전송
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildMonkey(mapManager.GetMapGimicIndex()));
    }

    // [Listener] 패킷 수신 시 실행
    public void ReceiveMonkeySync()
    {
        ResetCooldownsShared();
    }
}