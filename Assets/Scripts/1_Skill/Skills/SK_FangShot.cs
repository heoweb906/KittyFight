using UnityEngine;
using System.Collections;

public class SK_FangShot : Skill
{
    [Header("발사체 이동")]
    public float projectileSpeed = 12f;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 2);
        StartCoroutine(ResetAttackAnimState());

        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var proj = Instantiate(objSkillEntity, origin, rot);

        playerAbility.PlaySFX(sfxClip);

        var abBase = proj.GetComponent<AB_HitboxBase>();
        if (abBase != null) abBase.Init(playerAbility);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = direction * projectileSpeed;

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