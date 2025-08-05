using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_FeatherShot : Skill
{
    public SK_FeatherShot(PlayerAbility playerAbilty, SkillWorker skillWorker)
    : base(playerAbilty, skillWorker)
    {


    }

    protected override void ExecuteSkill()
    {
        Debug.Log("SK_FeatherShot!");

    }
}
