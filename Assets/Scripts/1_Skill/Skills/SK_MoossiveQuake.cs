using System.Collections;
using UnityEngine;

public class SK_MoossiveQuake : Skill
{
    [Header("타이밍")]
    [SerializeField] private float castDelay = 0.7f;

    [Header("스턴")]
    [SerializeField] private float stunDuration = 3f;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 5);

        if (objSkillEntity == null) return;
        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.identity,
            playerAbility.gameObject.transform
        );
        StartCoroutine(Co_Cast(origin));
    }

    private IEnumerator Co_Cast(Vector3 origin)
    {
        if (castDelay > 0f)
            yield return new WaitForSeconds(castDelay);

        Quaternion rot = Quaternion.identity;
        GameObject hitbox = Instantiate(objSkillEntity, origin, rot);

        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 6);
        StartCoroutine(ResetAttackAnimState());
    }
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}