using System.Collections;
using UnityEngine;

public class MapGimic_3_Tiger : AbstractMapGimic
{
    private readonly Color orangeColor = new Color(1f, 0.5f, 0f); // 주황색

    private Skill _p1OriginalRanged;
    private Skill _p2OriginalRanged;
    private bool _tigerApplied;

    public override void OnGimicStart()
    {
        base.OnGimicStart();

        if (mapManager != null)
        {
            mapManager.SetScreenColor(orangeColor);

            if (MatchResultStore.myPlayerNumber == 1)
            {
                P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Color(orangeColor));
                StartCoroutine(Co_TigerSequence());
            }
        }
    }

    public override void OnGimicEnd()
    {
        base.OnGimicEnd();

        if (MatchResultStore.myPlayerNumber == 1)
        {
            // 1. 화면 효과 리셋 전송
            P2PMessageSender.SendMessage(MapGimicBuilder.BuildScreen_Reset());

            // 2. [수정] 코루틴 없이 즉시 로직 해제 및 패킷 전송 (Rabbit 방식)
            UndoTigerLogic_Host();
        }
    }

    // [복구] 즉시 실행되는 종료 로직 함수
    private void UndoTigerLogic_Host()
    {
        Debug.Log("[Tiger] 호랑이 기믹 종료: 근거리 -> 원거리");

        // 내꺼 먼저 복구
        ChangeWeaponToRange();

        // 상대방에게 종료 패킷 즉시 전송
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildTiger(mapManager.GetMapGimicIndex(), false));
    }

    private IEnumerator Co_TigerSequence()
    {
        SendTween(0.2f, 0.4f);
        yield return new WaitForSeconds(0.25f);

        ExecuteTigerLogic_Host();

        SendTween(1f, 0.5f);
    }

    private void ExecuteTigerLogic_Host()
    {
        Debug.Log("[Tiger] 호랑이 기믹 발동");
        ChangeWeaponToMelee();
        P2PMessageSender.SendMessage(MapGimicBuilder.BuildTiger(mapManager.GetMapGimicIndex(), true));
    }

    public void ReceiveTigerSync(bool isStart)
    {
        if (isStart) ChangeWeaponToMelee();
        else ChangeWeaponToRange();
    }

    public void ChangeWeaponToMelee()
    {
        if (gameManager == null) return;
        if (_tigerApplied) return;

        var a1 = gameManager.playerAbility_1;
        var a2 = gameManager.playerAbility_2;
        if (a1 == null || a2 == null) return;

        _p1OriginalRanged = a1.rangedSkill;
        _p2OriginalRanged = a2.rangedSkill;

        a1.SetSkill(SkillType.Ranged, a1.meleeSkill);
        a2.SetSkill(SkillType.Ranged, a2.meleeSkill);

        a1.effect.PlayShakeAnimation(0);
        a2.effect.PlayShakeAnimation(0);


        _tigerApplied = true;
    }


    public void ChangeWeaponToRange()
    {
        if (gameManager == null) return;
        if (!_tigerApplied) return;

        var a1 = gameManager.playerAbility_1;
        var a2 = gameManager.playerAbility_2;
        if (a1 == null || a2 == null) return;

        if (_p1OriginalRanged != null) a1.SetSkill(SkillType.Ranged, _p1OriginalRanged);
        if (_p2OriginalRanged != null) a2.SetSkill(SkillType.Ranged, _p2OriginalRanged);

        a1.effect.PlayShakeAnimation(0);
        a2.effect.PlayShakeAnimation(0);

        _tigerApplied = false;
    }
}