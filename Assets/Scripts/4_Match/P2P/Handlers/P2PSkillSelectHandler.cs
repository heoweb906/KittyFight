using UnityEngine;

public class P2PSkillSelectHandler : IP2PMessageHandler
{
    private readonly SkillWorker skillWorkerOpponent;
    private readonly SkillCardController skillCardController;
    private readonly int myPlayerNumber;

    public P2PSkillSelectHandler(SkillWorker _worker, SkillCardController _skillCardController, int _myNumber)
    {
        skillWorkerOpponent = _worker;
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

        skillWorkerOpponent.EquipSkillByCard(objSkill);

        skillCardController.HideSkillCardList();
    }
}
