using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    void Execute();
}


public enum PlayerActionType
{
    SkillQ,
    SkillE
}


public class SkillActionManager : MonoBehaviour
{
    private Dictionary<PlayerActionType, IPlayerAction> actions = new Dictionary<PlayerActionType, IPlayerAction>();
    private PlayerAbility playerAbility;

    private void Start()
    {
        playerAbility = GetComponent<PlayerAbility>();

        SetAbilityReset();


    }

    public void RegisterAction(PlayerActionType type, IPlayerAction action)
    {
        actions[type] = action;
    }

    public void ExecuteAction(PlayerActionType type)
    {
        if (actions.ContainsKey(type))
        {
            actions[type].Execute();
        }
    }

    public void SetAbilityReset()
    {
        RegisterAction(PlayerActionType.SkillQ, new SkillAction(new Skill_Null(), playerAbility));
        RegisterAction(PlayerActionType.SkillE, new SkillAction(new Skill_Null(), playerAbility));
    }


    // ��ų 1���� �ο�
    public void SetSkill_1(Skill _skill)
    {
        RegisterAction(PlayerActionType.SkillQ, new SkillAction(_skill, playerAbility));
    }

    // ��ų 2���� �ο�
    public void SetSkill_2(Skill _skill)
    {
        RegisterAction(PlayerActionType.SkillE, new SkillAction(_skill, playerAbility));
    }
}
