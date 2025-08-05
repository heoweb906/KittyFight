using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_HoofOut : Skill
{
    public float maxRange = 3.0f;
    public SK_HoofOut(PlayerAbility playerAbilty, SkillWorker skillWorker)
    : base(playerAbilty, skillWorker)
    {


    }

    protected override void ExecuteSkill()
    {
        Debug.Log("HoofOut launched!");

        Vector3 aimPos, dir;
        AttackUtils.GetAimPointAndDirection(playerAbilty.transform, maxRange, out aimPos, out dir);

        Vector3 spawnPos = playerAbilty.transform.position + dir * maxRange;
        Quaternion rot = Quaternion.LookRotation(dir);

        if (objSkillEntity != null)
        {
            GameObject hitbox = Instantiate(objSkillEntity, spawnPos, rot);
            var ab = hitbox.GetComponent<AB_HoofOut>();
            if (ab != null)
            {
                ab.ownerPlayerNumber = playerAbilty.playerNumber;
            }
            Destroy(hitbox, 1.0f);
        }
    }
}
