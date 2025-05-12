using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Skill
{
    public FireBall(PlayerAbility playerAbilty, PlayerSkillAbilty skillAbilty)
     : base(playerAbilty, skillAbilty)
    {


    }

    protected override void ExecuteSkill()
    {
        Debug.Log("Fireball launched!");
        if (objSkillEntity != null)
        {

            GameObject.Instantiate(objSkillEntity);
        }
    }
}
