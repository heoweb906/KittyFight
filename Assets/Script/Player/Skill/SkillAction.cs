using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAction : IPlayerAction
{
    private Skill skill;
    private PlayerAbility ability;

    public SkillAction(Skill skill, PlayerAbility _ability)
    {
        this.skill = skill;
        this.ability = _ability;
    }




    public void Execute()
    {
        skill.Activate(ability);
    }


}
