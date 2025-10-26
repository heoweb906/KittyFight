using UnityEngine;

public class SK_FangShot : Skill
{
    [Header("�߻�ü �̵�")]
    public float projectileSpeed = 12f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var proj = Instantiate(objSkillEntity, origin, rot);

        var abBase = proj.GetComponent<AB_HitboxBase>();
        if (abBase != null) abBase.Init(playerAbility);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = direction * projectileSpeed;
    }
}