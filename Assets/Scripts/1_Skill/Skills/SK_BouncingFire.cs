using UnityEngine;
using System.Collections;

public class SK_BouncingFire : Skill
{
    [Header("투사체 설정")]
    [SerializeField] private float projectileSpeed = 10f;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;
    private Animator anim;

    private void Awake()
    {
        anim = playerAbility.GetComponentInChildren<Animator>();
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;
        if (!objSkillEntity) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 2);
        StartCoroutine(ResetAttackAnimState());

        Vector3 spawnPos = origin + direction * aimRange;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        GameObject proj = Instantiate(objSkillEntity, spawnPos, rot);

        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = direction * projectileSpeed;

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
    }
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}