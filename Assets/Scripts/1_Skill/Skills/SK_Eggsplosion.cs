using UnityEngine;

public class SK_Eggsplosion : Skill
{
    public float projectileSpeed = 20f;
    public float spawnOffset = 0.6f;
    public float spreadAngleDeg = 25f;

    private void Awake()
    {
        coolTime = 6.0f;
        aimRange = 3.0f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

        Vector3 dirC = direction.normalized;
        Vector3 dirL = Quaternion.AngleAxis(-spreadAngleDeg, Vector3.up) * dirC;
        Vector3 dirR = Quaternion.AngleAxis(+spreadAngleDeg, Vector3.up) * dirC;

        FireOne(origin, dirL);
        FireOne(origin, dirC);
        FireOne(origin, dirR);
    }

    private void FireOne(Vector3 origin, Vector3 dir)
    {
        Vector3 spawnPos = origin + dir * spawnOffset;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        var proj = Object.Instantiate(objSkillEntity, spawnPos, rot);

        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.velocity = dir * projectileSpeed;
        }
    }
}