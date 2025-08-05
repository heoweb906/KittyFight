using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_FlameBurst : Skill
{
    public SK_FlameBurst(PlayerAbility playerAbilty, SkillWorker skillWorker)
    : base(playerAbilty, skillWorker)
    {


    }

    protected override void ExecuteSkill()
    {
        Debug.Log("FlameBurst launched!");

    }
}
