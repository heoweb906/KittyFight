using UnityEngine;
using System.Collections;

public class SK_ShearShock : Skill
{
    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());

        var rot = Quaternion.LookRotation(direction, Vector3.up);
        Vector3 spawnPos = origin + rot * Vector3.right * -0.5f;
        var shock = Instantiate(objSkillEntity, spawnPos, rot);
        var ab = shock.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        playerAbility.PlaySFX(sfxClip);

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