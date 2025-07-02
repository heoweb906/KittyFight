using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_WindStep : Skill
{
    public SK_WindStep(PlayerAbility playerAbilty, SkillWorker skillWorker)
    : base(playerAbilty, skillWorker)
    {


    }

    protected override void ExecuteSkill()
    {
        Debug.Log("WindStep launched!");

    }
}
