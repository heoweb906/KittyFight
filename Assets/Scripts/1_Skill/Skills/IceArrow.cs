using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : Skill
{
    public IceArrow(PlayerAbility playerAbilty, SkillWorker skillWorker)
    : base(playerAbilty, skillWorker)
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
