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

        GameObject objSkill = skillCardController.CreateSkillInstance(model.skillCard_SO);

        if (model.skillCard_SO.cardType == CardType.Active)
        {
            // ��Ƽ�� ��ų ó��
            Skill skillComponent = objSkill.GetComponent<Skill>();
            if (skillComponent != null)
            {
                SkillType targetSlot = playerAbilityOpponent.GetSkill(SkillType.Skill1) == null ? SkillType.Skill1 : SkillType.Skill2;
                playerAbilityOpponent.SetSkill(targetSlot, skillComponent);
            }
        }
        else if (model.skillCard_SO.cardType == CardType.Passive)
        {
            // �нú� ��ų ó��
            Passive passiveComponent = objSkill.GetComponent<Passive>();
            if (passiveComponent != null)
            {
                playerAbilityOpponent.EquipPassive(passiveComponent);
            }
        }


        skillCardController.SetBoolAllCardInteract(false);
        skillCardController.iAuthorityPlayerNum = 0;


        if (model.bIsRat)
        {
            skillCardController.HIdeSkillCardList_ForRat(model.skillCard_SO.iAnimalNum, model.cardPosition, model.iRandomSkillIndex);
        }
        else
        {
            skillCardController.HideSkillCardList(model.skillCard_SO.iAnimalNum, model.cardPosition);
        }
    }
}