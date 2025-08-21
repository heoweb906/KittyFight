using UnityEngine;

public class SK_Rend : Skill
{
    private void Awake()
    {
        coolTime = 4.5f;
        aimRange = 3.0f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var go = Instantiate(objSkillEntity, origin, rot);

        var ab = go.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);
    }
}