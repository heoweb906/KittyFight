using UnityEngine;

public class SK_ShearShock : Skill
{
    private void Awake()
    {
        coolTime = 6.5f;
        aimRange = 2.5f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var shock = Instantiate(objSkillEntity, origin, rot);

        var ab = shock.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);
    }
}