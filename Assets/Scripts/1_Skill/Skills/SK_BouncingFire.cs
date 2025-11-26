using UnityEngine;

public class SK_BouncingFire : Skill
{
    [Header("투사체 설정")]
    [SerializeField] private float projectileSpeed = 10f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;
        if (!objSkillEntity) return;

        Vector3 spawnPos = origin + direction * aimRange;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        GameObject proj = Instantiate(objSkillEntity, spawnPos, rot);

        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = direction * projectileSpeed;
    }
}