using UnityEngine;

public class P2PSkillSelectHandler : IP2PMessageHandler
{
    private readonly PlayerAbility playerAbilityOpponent;
    private readonly SkillCardController skillCardController;
    private readonly int myPlayerNumber;

    public P2PSkillSelectHandler(PlayerAbility playerAbility, SkillCardController skillCardController, int myNumber)
    {
        playerAbilityOpponent = playerAbility;
        this.skillCardController = skillCardController;
        myPlayerNumber = myNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[SKILL_SELECT]");

    public void Handle(string msg)
    {
        var model = JsonUtility.FromJson<Model_SkillSelect>(msg.Substring("[SKILL_SELECT]".Length));
        if (model.iPlayer == myPlayerNumber) return;

        // =================================================================================
        // 1. [장착용 데이터 결정] 
        // 쥐라면 '숨겨진 스킬(RandomIndex)'을 장착하고, 아니면 '보낸 스킬(SkillIndex)'을 장착
        // =================================================================================
        int equipSkillID = model.bIsRat ? model.iRandomSkillIndex : model.iSkillIndex;

        SkillCard_SO skillToEquip = skillCardController.FindSkillCardByIndex(equipSkillID);

        if (skillToEquip == null)
        {
            Debug.LogError($"[P2P] 스킬을 찾을 수 없습니다. ID: {equipSkillID}");
            return;
        }

        // 스킬 생성 및 장착 (실제 게임 로직용)
        GameObject objSkill = skillCardController.CreateSkillInstance(skillToEquip);

        if (skillToEquip.cardType == CardType.Active)
        {
            Skill skillComponent = objSkill.GetComponent<Skill>();
            if (skillComponent != null)
            {
                SkillType targetSlot = playerAbilityOpponent.GetSkill(SkillType.Skill1) == null ? SkillType.Skill1 : SkillType.Skill2;
                playerAbilityOpponent.SetSkill(targetSlot, skillComponent);
            }
        }
        else if (skillToEquip.cardType == CardType.Passive)
        {
            Passive passiveComponent = objSkill.GetComponent<Passive>();
            if (passiveComponent != null)
            {
                playerAbilityOpponent.EquipPassive(passiveComponent);
            }
        }

        skillCardController.SetBoolAllCardInteract(false);
        skillCardController.iAuthorityPlayerNum = 0;

        // =================================================================================
        // 2. [연출 실행]
        // 쥐라면: '껍데기(model.iSkillIndex)'를 보여주고 -> 나중에 '알맹이(model.iRandomSkillIndex)' 획득
        // =================================================================================
        if (model.bIsRat)
        {
            // 좌표 변환
            Transform canvasTransform = skillCardController.InGameUiController.canvasMain.transform;
            Vector3 worldPos = canvasTransform.TransformPoint(model.cardPosition);

            // 1번째 인자: model.iSkillIndex (보낸 사람이 클릭한 '호랑이' ID)
            // 3번째 인자: model.iRandomSkillIndex (실제 획득한 '쥐' ID)
            skillCardController.HIdeSkillCardList_ForRat(
                model.iSkillIndex,
                worldPos,
                model.iRandomSkillIndex
            );
        }
        else
        {
            // 쥐가 아니면 그냥 해당 스킬 연출
            skillCardController.HideSkillCardList(model.iSkillIndex, model.cardPosition);
        }
    }
}