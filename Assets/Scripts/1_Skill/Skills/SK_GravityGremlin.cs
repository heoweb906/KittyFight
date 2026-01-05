using UnityEngine;
using System.Collections;
public class SK_GravityGremlin : Skill
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 14f;

    [Header("Blackhole Settings")]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float blackholeDuration = 6f;

    [Header("Camera FX")]
    [SerializeField] private float shakeStrength = 0.25f;
    [SerializeField] private float shakeDuration = 0.35f;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration_camera;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;
        if (!objSkillEntity) return;
        if (!blackholePrefab) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());

        direction.z = 0f;
        origin.z = playerAbility.transform.position.z;

        if (direction.sqrMagnitude < 0.001f)
            direction = playerAbility.transform.right;

        direction.Normalize();

        // 투사체 생성
        GameObject proj = Instantiate(
            objSkillEntity,
            origin,
            Quaternion.LookRotation(direction, Vector3.up)
        );

        playerAbility.PlaySFX(sfxClip);

        var projHitbox = proj.GetComponent<AB_GravityGremlin>();
        if (projHitbox == null)
        {
            projHitbox = proj.AddComponent<AB_GravityGremlin>();
        }

        projHitbox.InitProjectile(
            playerAbility,
            blackholePrefab,
            blackholeDuration,
            shakeStrength,
            shakeDuration
        );

        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = direction * projectileSpeed;
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
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}