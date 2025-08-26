using UnityEngine;
public class P2PSkillShowHandler : IP2PMessageHandler
{
    private readonly SkillCardController skillCardController;
    private readonly int myPlayerNumber;
    public P2PSkillShowHandler(SkillCardController skillCardController, int myNumber)
    {
        this.skillCardController = skillCardController;
        myPlayerNumber = myNumber;
    }
    public bool CanHandle(string msg) => msg.StartsWith("[SKILL_SHOW]");
    public void Handle(string msg)
    {
        var model = JsonUtility.FromJson<Model_SkillShow>(msg.Substring("[SKILL_SHOW]".Length));
        if (model.iPlayer == myPlayerNumber) return;

        if (MatchResultStore.myPlayerNumber == 1)
            skillCardController.ShowSkillCardList(2, true, model.iArraySkillCardNum);
        else
            skillCardController.ShowSkillCardList(1, true, model.iArraySkillCardNum);
    }
}