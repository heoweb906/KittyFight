using System.Collections;
using UnityEngine;

public class MapGimic_4_Rabbit : AbstractMapGimic
{
    // 주황(1, 0.5, 0) + 초록(0, 1, 0)의 중간색 = 라임/연두색 (0.5, 0.75, 0)
    private readonly Color rabbitColor = new Color(0.5f, 0.75f, 0f);

    private Vector3 originalGravity; // 복구용 저장 변수

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(rabbitColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(rabbitColor));
                StartCoroutine(Co_RabbitSequence());
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        if (MatchResultStore.myPlayerNumber == 1)
        {
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
            UndoRabbitLogic_Host();
        }
    }

    private IEnumerator Co_RabbitSequence()
    {
        SendTween(0.2f, 0.4f);
        yield return new WaitForSeconds(0.25f);

        ExecuteRabbitLogic_Host();

        SendTween(1f, 0.5f);
    }

    private void ExecuteRabbitLogic_Host()
    {
        ChangeGravityLow();
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildRabbit(mapManager.GetMapGimicIndex(), true));
    }

    private void UndoRabbitLogic_Host()
    {
        RestoreGravity();
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildRabbit(mapManager.GetMapGimicIndex(), false));
    }

    public void ReceiveRabbitSync(bool isStart)
    {
        if (isStart)
            ChangeGravityLow();
        else
            RestoreGravity();
    }

    // --- 기능 구현 ---
    private void ChangeGravityLow()
    {
        // 현재 중력 저장
        originalGravity = Physics.gravity;
        // 절반으로 적용
        Physics.gravity = originalGravity * 0.45f;
    }

    private void RestoreGravity()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
    }
}