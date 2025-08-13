using UnityEngine;
public class P2PSkillSelectHandler : IP2PMessageHandler
{
    private readonly PlayerAbility playerAbilityOpponent;
    private readonly SkillCardController skillCardController;
    private readonly int myPlayerNumber;
    public P2PSkillSelectHandler(PlayerAbility _playerAbility, SkillCardController _skillCardController, int _myNumber)
    {
        playerAbilityOpponent = _playerAbility;
        skillCardController = _skillCardController;
        myPlayerNumber = _myNumber;
    }
    public bool CanHandle(string msg) => msg.StartsWith("[SKILL_SELECT]");
    public void Handle(string msg)
    {
        var model = JsonUtility.FromJson<Model_SkillSelect>(msg.Substring("[SKILL_SELECT]".Length));
        if (model.iPlayer == myPlayerNumber) return;
        SkillCard_SO so_SkillCard = skillCardController.skillDataList.Find(card => card.sSkillName == model.sSkillCardName);
        if (so_SkillCard == null)
        {
            Debug.LogError($"[P2PSkillSelectHandler] SkillCard_SO '{model.sSkillCardName}' not found.");
            return;
        }
        GameObject objSkill = skillCardController.CreateSkillInstance(so_SkillCard);
        if (objSkill == null)
        {
            Debug.LogError("[P2PSkillSelectHandler] Failed to create skill prefab.");
            return;
        }

        Skill skillComponent = objSkill.GetComponent<Skill>();
        if (skillComponent != null)
        {
            // Skill1 ¶Ç´Â Skill2 ½½·Ô¿¡ ÀåÂø (ºó ½½·Ô ¿ì¼±)
            SkillType targetSlot = playerAbilityOpponent.GetSkill(SkillType.Skill1) == null ? SkillType.Skill1 : SkillType.Skill2;
            playerAbilityOpponent.SetSkill(targetSlot, skillComponent);
        }

        skillCardController.HideSkillCardList();
    }
}