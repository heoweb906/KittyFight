using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGimic_11_Dog : AbstractMapGimic
{
    // 검붉은 색 (Dark Red)
    private readonly Color dogColor = new Color(0.5f, 0.1f, 0f);

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(dogColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(dogColor));
                StartCoroutine(Co_DogSequence());
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        if (MatchResultStore.myPlayerNumber == 1)
        {
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
            UndoDogLogic_Host();
        }
    }

    private IEnumerator Co_DogSequence()
    {
        SendTween(0.2f, 0.45f);
        yield return new WaitForSeconds(0.25f);

        ExecuteDogLogic_Host();

        SendTween(1f, 0.5f);
    }

    private void ExecuteDogLogic_Host()
    {
        SetDogGimickActive(true);
        // 11번 기믹 패킷 전송 (Packet_11_Dog 및 Builder 구현 필요)
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildDog(mapManager.GetMapGimicIndex(), true));
    }

    private void UndoDogLogic_Host()
    {
        SetDogGimickActive(false);
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildDog(mapManager.GetMapGimicIndex(), false));
    }

    public void ReceiveDogSync(bool isActive)
    {
        SetDogGimickActive(isActive);
    }

    private void SetDogGimickActive(bool isActive)
    {
        // 내 로컬 플레이어의 Health 컴포넌트 찾기
        var myHealth = (MatchResultStore.myPlayerNumber == 1)
            ? mapManager.gameManager.playerAbility_1.GetComponent<PlayerHealth>()
            : mapManager.gameManager.playerAbility_2.GetComponent<PlayerHealth>();

        if (myHealth != null)
        {
            myHealth.bDogGimickOn = isActive;
        }
    }
}