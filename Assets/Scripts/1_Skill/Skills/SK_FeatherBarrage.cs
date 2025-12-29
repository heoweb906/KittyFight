using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_FeatherBarrage : Skill
{
    [Header("발사 설정")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private int projectileCount = 12;
    [SerializeField] private GameObject effectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;
        if (!playerAbility) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);
        StartCoroutine(ResetAttackAnimState());

        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.identity
        );

        Vector3 center = playerAbility.transform.position;
        center.z = playerAbility.transform.position.z;

        Vector3 baseDir = Vector3.right;
        baseDir.z = 0f;
        baseDir.Normalize();

        float angleStep = 360f / projectileCount;

        List<Collider> group = new List<Collider>();

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = angleStep * i;

            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * baseDir;
            dir.z = 0f;
            dir.Normalize();

            var col = FireOne(center, dir);
            if (col != null)
                group.Add(col);
        }

        for (int i = 0; i < group.Count; i++)
        {
            for (int j = i + 1; j < group.Count; j++)
            {
                if (group[i] != null && group[j] != null)
                {
                    Physics.IgnoreCollision(group[i], group[j]);
                }
            }
        }

        var gm = FindObjectOfType<GameManager>();
        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }
    }

    private Collider FireOne(Vector3 center, Vector3 dir)
    {
        Vector3 spawnPos = center + dir * aimRange;

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        var proj = Object.Instantiate(objSkillEntity, spawnPos, rot);

        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.velocity = dir * projectileSpeed;
        }

        return proj.GetComponent<Collider>();
    }
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}