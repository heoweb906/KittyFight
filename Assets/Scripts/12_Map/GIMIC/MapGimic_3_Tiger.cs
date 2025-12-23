using System.Collections;
using UnityEngine;

public class MapGimic_3_Tiger : AbstractMapGimic
{
    private readonly Color orangeColor = new Color(1f, 0.5f, 0f); // 주황색

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            // 1. 색상 설정 (주황색)
            mapManager.SetScreenColor(orangeColor);

            // 2. 호스트만 연출 시퀀스 시작 및 색상 동기화
            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(orangeColor));
                StartCoroutine(Co_TigerSequence());
            }
        }
    }

    // 빈 Update 함수 삭제함

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        if (MatchResultStore.myPlayerNumber == 1)
        {
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());
            UndoTigerLogic_Host(); // ★ 종료 시 로직 되돌리기 호출
        }
    }

    private IEnumerator Co_TigerSequence()
    {
        // 1. 쾅! (0.2초만에 0.4f)
        SendTween(0.2f, 0.4f);
        yield return new WaitForSeconds(0.25f);

        // 2. 로직 실행 (원거리 -> 근거리)
        ExecuteTigerLogic_Host();

        // 3. 복귀 (1초만에 0.5f)
        SendTween(1f, 0.5f);
    }

    private void ExecuteTigerLogic_Host()
    {
        Debug.Log("[Tiger] 호랑이 기믹 발동: 원거리 -> 근거리");

        // 내꺼 변경 로직 (함수로 분리)
        ChangeWeaponToMelee();

        // 패킷 전송 (true = Start)
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildTiger(mapManager.GetMapGimicIndex(), true));
    }

    private void UndoTigerLogic_Host()
    {
        Debug.Log("[Tiger] 호랑이 기믹 종료: 근거리 -> 원거리");
        // 내꺼 복구 로직
        ChangeWeaponToRange();

        // 패킷 전송 (false = End)
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildTiger(mapManager.GetMapGimicIndex(), false));
    }


    public void ReceiveTigerSync(bool isStart)
    {
        if (isStart)
        {
            ChangeWeaponToMelee(); // 시작 로직
        }
        else
        {
            ChangeWeaponToRange(); // 종료 로직
        }
    }




    // --- 실제 기능 구현부 ---
    // --- 실제 기능 구현부 ---
    // --- 실제 기능 구현부 ---
    private void ChangeWeaponToMelee()
    {
        // 실제 무기 변경 코드
    }

    private void ChangeWeaponToRange()
    {
        // 실제 무기 복구 코드
    }
}