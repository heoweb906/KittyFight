using UnityEngine;
public class SK_ShearShock : Skill
{
    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;
        var rot = Quaternion.LookRotation(direction, Vector3.up);
        Vector3 spawnPos = origin + rot * Vector3.right * -0.5f;
        var shock = Instantiate(objSkillEntity, spawnPos, rot);
        var ab = shock.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);
    }
}