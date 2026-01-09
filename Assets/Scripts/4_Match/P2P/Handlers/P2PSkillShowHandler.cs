using System;
using UnityEngine;

public class P2PSkillShowHandler : IP2PMessageHandler
{
    private const string Prefix = "[SKILL_SHOW]";

    private readonly SkillCardController skillCardController;
    private readonly int myPlayerNumber;

    public P2PSkillShowHandler(SkillCardController skillCardController, int myNumber)
    {
        this.skillCardController = skillCardController;
        myPlayerNumber = myNumber;
    }

    public bool CanHandle(string msg) => !string.IsNullOrEmpty(msg) && msg.StartsWith(Prefix);

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;
        if (string.IsNullOrEmpty(msg)) return;

        if (!msg.StartsWith(Prefix)) return;
        if (msg.Length <= Prefix.Length) return;

        if (skillCardController == null)
        {
            Debug.LogWarning("[P2P][SKILL_SHOW] skillCardController is null. Skip.");
            return;
        }

        string json = msg.Substring(Prefix.Length);
        if (string.IsNullOrWhiteSpace(json)) return;

        Model_SkillShow model;
        try
        {
            model = JsonUtility.FromJson<Model_SkillShow>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][SKILL_SHOW] JSON parse failed. msgLen={msg.Length} err={e.Message}");
            return;
        }

        if (model == null) return;
        if (model.iPlayer == myPlayerNumber) return;

        if (MatchResultStore.myPlayerNumber == 1)
            skillCardController.ShowSkillCardList(2, true, model.iArraySkillCardNum);
        else
            skillCardController.ShowSkillCardList(1, true, model.iArraySkillCardNum);
    }
}