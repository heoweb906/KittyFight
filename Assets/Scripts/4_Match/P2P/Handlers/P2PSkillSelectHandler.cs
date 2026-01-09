using System;
using UnityEngine;

public class P2PSkillSelectHandler : IP2PMessageHandler
{
    private const string Prefix = "[SKILL_SELECT]";

    private readonly PlayerAbility playerAbilityOpponent;
    private readonly SkillCardController skillCardController;
    private readonly int myPlayerNumber;

    public P2PSkillSelectHandler(PlayerAbility playerAbility, SkillCardController skillCardController, int myNumber)
    {
        playerAbilityOpponent = playerAbility;
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

        string json = msg.Substring(Prefix.Length);
        if (string.IsNullOrWhiteSpace(json)) return;

        Model_SkillSelect model;
        try
        {
            model = JsonUtility.FromJson<Model_SkillSelect>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][SKILL_SELECT] JSON parse failed. msgLen={msg.Length} err={e.Message}");
            return;
        }

        if (model == null) return;
        if (model.iPlayer == myPlayerNumber) return;

        if (skillCardController == null)
        {
            Debug.LogWarning("[P2P][SKILL_SELECT] skillCardController is null. Skip.");
            return;
        }

        if (playerAbilityOpponent == null)
        {
            Debug.LogWarning("[P2P][SKILL_SELECT] playerAbilityOpponent is null. Skip equip.");
            return;
        }

        int equipSkillID = model.bIsRat ? model.iRandomSkillIndex : model.iSkillIndex;

        skillCardController.MarkSkillAsUsed(equipSkillID);

        SkillCard_SO skillToEquip = skillCardController.FindSkillCardByIndex(equipSkillID);
        if (skillToEquip == null)
        {
            Debug.LogError($"[P2P][SKILL_SELECT] SkillCard not found. ID: {equipSkillID}");
            return;
        }

        GameObject objSkill = skillCardController.CreateSkillInstance(skillToEquip);
        if (objSkill == null)
        {
            Debug.LogWarning($"[P2P][SKILL_SELECT] CreateSkillInstance returned null. ID: {equipSkillID}");
            return;
        }

        if (skillToEquip.cardType == CardType.Active)
        {
            Skill skillComponent = objSkill.GetComponent<Skill>();
            if (skillComponent != null)
            {
                SkillType targetSlot =
                    playerAbilityOpponent.GetSkill(SkillType.Skill1) == null ? SkillType.Skill1 : SkillType.Skill2;

                playerAbilityOpponent.SetSkill(targetSlot, skillComponent);
            }
            else
            {
                Debug.LogWarning($"[P2P][SKILL_SELECT] Skill component missing on instance. ID: {equipSkillID}");
            }
        }
        else if (skillToEquip.cardType == CardType.Passive)
        {
            Passive passiveComponent = objSkill.GetComponent<Passive>();
            if (passiveComponent != null)
            {
                playerAbilityOpponent.EquipPassive(passiveComponent);
            }
            else
            {
                Debug.LogWarning($"[P2P][SKILL_SELECT] Passive component missing on instance. ID: {equipSkillID}");
            }
        }

        skillCardController.SetBoolAllCardInteract(false);
        skillCardController.iAuthorityPlayerNum = 0;

        if (model.bIsRat)
        {
            var ui = skillCardController.InGameUiController;
            var canvas = ui != null ? ui.canvasMain : null;

            if (canvas != null)
            {
                Transform canvasTransform = canvas.transform;
                Vector3 worldPos = canvasTransform.TransformPoint(model.cardPosition);

                skillCardController.HIdeSkillCardList_ForRat(
                    model.iSkillIndex,
                    worldPos,
                    model.iRandomSkillIndex
                );
            }
            else
            {
                Debug.LogWarning("[P2P][SKILL_SELECT] canvasMain is null. Skip RAT visual.");
            }
        }
        else
        {
            skillCardController.HideSkillCardList(model.iSkillIndex, model.cardPosition);
        }
    }
}