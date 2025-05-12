using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : Skill
{
    public IceArrow(PlayerAbility playerAbilty, PlayerSkillAbilty skillAbilty)
    : base(playerAbilty, skillAbilty)
    {


    }

    protected override void ExecuteSkill()
    {
        Debug.Log("IceArrow launched!");
        if (objSkillEntity != null)
        {

            GameObject.Instantiate(objSkillEntity);
        }
    }
}
