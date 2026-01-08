using System.Collections;
using UnityEngine;

public class MapGimic_1_Rat : AbstractMapGimic
{
    [Header("Gimmick Settings")]
    [SerializeField] private float activationInterval = 12.0f; // 12초 주기로 변경

    private float timer;
    private bool isSequenceStarted = false; // 시퀀스 중복 실행 방지용

    public override void OnGimicStart()
    {
        base.OnGimicStart();
        timer = 0f;
        isSequenceStarted = false;

        if (mapManager != null)
        {
            // [수정] 호스트가 색상 변경 시 패킷도 전송
            mapManager.SetScreenColor(Color.red);
            if (MatchResultStore.myPlayerNumber == 1)
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(Color.red));
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd(); // 부모에서 ResetScreenEffect 호출함

        // [추가] 종료 시 리셋 패킷 전송 (Host만)
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
                StartCoroutine(Co_PlayRatSequence());
            }

            // 사이클 초기화 (12초 지남)
            if (timer >= activationInterval)
            {
                timer = 0f;
                isSequenceStarted = false;
            }
        }
    }

    private IEnumerator Co_PlayRatSequence()
    {
        // [수정] 각 단계마다 로컬 실행 + 패킷 전송

        // 1단계
        SendTween(1f, 0.44f);
        yield return new WaitForSeconds(1f);

        // 2단계
        SendTween(1f, 0.5f);
        yield return new WaitForSeconds(1f);

        // 3단계
        SendTween(1f, 0.44f);
        yield return new WaitForSeconds(1f);

        // 4단계
        SendTween(1f, 0.5f);
        yield return new WaitForSeconds(1f);


        SendTween(0.15f, 0.4f);
        DistributeHealth_Host();
        yield return new WaitForSeconds(0.15f);
        SendTween(0.15f, 0.5f);
    }



    // [Host 전용] 로직 (기존 유지)
    private void DistributeHealth_Host()
    {
        if (!gameManager.playerAbility_1 || !gameManager.playerAbility_2) return;

        var p1Health = gameManager.playerAbility_1.GetComponent<PlayerHealth>();
        var p2Health = gameManager.playerAbility_2.GetComponent<PlayerHealth>();

        if (p1Health == null || p2Health == null) return;

        int hp1 = p1Health.CurrentHP;
        int hp2 = p2Health.CurrentHP;
        int totalHP = hp1 + hp2;
        int averageHP = totalHP / 2;

        if (hp1 == averageHP && hp2 == averageHP) return;

        //ApplyHP(p1Health, averageHP);
        p1Health.RemoteSetHP(averageHP);
        p2Health.RemoteSetHP(averageHP);

        string packet = MapGimicBuilder.BuildRat_SyncHP(gameManager.mapManager.GetMapGimicIndex(), averageHP);
        P2PMessageSender.SendMessage(packet);
    }

    public void ReceiveSyncHP(int targetHP)
    {
        //var p2Health = gameManager.playerAbility_2.GetComponent<PlayerHealth>();
        //if (p2Health != null)
        //{
        //    ApplyHP(p2Health, targetHP);
        //}

        var p1 = gameManager.playerAbility_1.GetComponent<PlayerHealth>();
        var p2 = gameManager.playerAbility_2.GetComponent<PlayerHealth>();

        if (p1 != null) p1.RemoteSetHP(targetHP);
        if (p2 != null) p2.RemoteSetHP(targetHP);
    }

    private void ApplyHP(PlayerHealth target, int targetValue)
    {
        int current = target.CurrentHP;
        int diff = current - targetValue;

        if (diff > 0) target.ForceDamage(diff);
        else if (diff < 0) target.Heal(Mathf.Abs(diff));
    }
}