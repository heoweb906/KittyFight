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
 
        Skill skillComponent = objSkill.GetComponent<Skill>();
        if (skillComponent != null)
        {
            SkillType targetSlot = playerAbilityOpponent.GetSkill(SkillType.Skill1) == null ? SkillType.Skill1 : SkillType.Skill2;
            playerAbilityOpponent.SetSkill(targetSlot, skillComponent);
        }

        skillCardController.HideSkillCardList(model.skillCard_SO.iAnimalNum, model.cardPosition);
    }
}