using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_StoneSheild : Skill
{
    public SK_StoneSheild(PlayerAbility playerAbilty, SkillWorker skillWorker)
    : base(playerAbilty, skillWorker)
    {


    }

    protected override void ExecuteSkill()
    {
        Debug.Log("StoneSheild launched!");

    }
}
