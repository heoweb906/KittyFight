using UnityEngine;
using System.Collections;

public class SK_SavageRoar : Skill
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
        var go = Instantiate(objSkillEntity, origin, rot);

        var hitboxes = go.GetComponentsInChildren<AB_HitboxBase>(true);
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i].Init(playerAbility);
        }

        var gm = FindObjectOfType<GameManager>();
        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeAmount, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeAmount * 0.5f, direction);
        }
    }
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}
