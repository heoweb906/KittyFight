using UnityEngine;

public class P2PSkillShowHandler : IP2PMessageHandler
{
    private readonly SkillCardController skillCardController;
    private readonly int myPlayerNumber;


    public P2PSkillShowHandler(SkillCardController _skillCardController, int _myNumber)
    {
        skillCardController = _skillCardController;
        myPlayerNumber = _myNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[SKILL_SHOW]");

    public void Handle(string msg)
    {
        var model = JsonUtility.FromJson<Model_SkillSelect>(msg.Substring("[SKILL_SHOW]".Length));
        if (model.iPlayer == myPlayerNumber) return;


        if(MatchResultStore.myPlayerNumber == 1) skillCardController.ShowSkillCardList(2);
        else skillCardController.ShowSkillCardList(1);
    }
}
