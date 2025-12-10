using System.Collections.Generic;
using UnityEngine;
public class SK_FeatherShot : Skill
{
    [Header("발사 설정")]
    public float projectileSpeed = 20f;
    public float spawnOffset = 0.6f;
    public float spreadAngleDeg = 20f;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;
        Vector3 dirC = direction.normalized;
        Vector3 dirL = Quaternion.AngleAxis(+spreadAngleDeg, Vector3.forward) * dirC;
        Vector3 dirR = Quaternion.AngleAxis(-spreadAngleDeg, Vector3.forward) * dirC;

        List<Collider> throwGroup = new List<Collider>();
        throwGroup.Add(FireOne(origin, dirL));
        throwGroup.Add(FireOne(origin, dirC));
        throwGroup.Add(FireOne(origin, dirR));

        for (int i = 0; i < throwGroup.Count; i++)
        {
            for (int j = i + 1; j < throwGroup.Count; j++)
            {
                if (throwGroup[i] != null && throwGroup[j] != null)
                {
                    Physics.IgnoreCollision(throwGroup[i], throwGroup[j]);
                }
            }
        }

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
    }
    private Collider FireOne(Vector3 origin, Vector3 dir)
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
        return proj.GetComponent<Collider>();
    }
}