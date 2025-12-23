using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGimic_7_Horse : AbstractMapGimic
{
    // 남색 (Navy)
    private readonly Color horseColor = new Color(0f, 0f, 0.5f);

    private float originalMoveSpeed;
    private PlayerAbility myAbility;

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(horseColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(horseColor));
                StartCoroutine(Co_HorseSequence());
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        if (MatchResultStore.myPlayerNumber == 1)
        {
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
            UndoHorseLogic_Host();
        }
    }

    private IEnumerator Co_HorseSequence()
    {
        SendTween(0.2f, 0.4f);
        yield return new WaitForSeconds(0.25f);

        ExecuteHorseLogic_Host();

        SendTween(1f, 0.5f);
    }

    private void ExecuteHorseLogic_Host()
    {
        ApplySpeedBoost();
        // 7번 기믹 패킷 전송
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildHorse(mapManager.GetMapGimicIndex(), true));
    }

    private void UndoHorseLogic_Host()
    {
        RestoreSpeed();
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildHorse(mapManager.GetMapGimicIndex(), false));
    }

    public void ReceiveHorseSync(bool isStart)
    {
        if (isStart)
            ApplySpeedBoost();
        else
            RestoreSpeed();
    }

    // --- 실제 기능 구현부 ---
    private void ApplySpeedBoost()
    {
        // 내 로컬 플레이어 찾기 (GameManager의 참조 활용)
        myAbility = (MatchResultStore.myPlayerNumber == 1)
            ? mapManager.gameManager.playerAbility_1
            : mapManager.gameManager.playerAbility_2;

        if (myAbility != null)
        {
            originalMoveSpeed = myAbility.moveSpeed; // 실행 시점의 속도 저장
            myAbility.moveSpeed *= 1.5f;             // 1.5배 증가
        }
    }

    private void RestoreSpeed()
    {
        if (myAbility != null)
        {
            myAbility.moveSpeed = originalMoveSpeed; // 저장했던 속도로 복구
        }
    }
}