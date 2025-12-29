using UnityEngine;
using System.Collections;

public class SK_HoofOut : Skill
{

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (objSkillEntity == null) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());

        Vector3 spawnPos = origin;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        GameObject hitbox = Instantiate(objSkillEntity, spawnPos, rot);

        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        // HoofOut 전용: 스폰 시점에 좌/우 부호 확정해서 넘김
        var hoof = hitbox.GetComponent<AB_HoofOut>();
        if (hoof != null)
        {
            // 1) 기본: 조준 방향의 x 성분으로 좌/우 판정
            float sign = Mathf.Abs(direction.x) > 0.0001f ? Mathf.Sign(direction.x) : 0f;

            // 2) 만약 x가 거의 0이면(정면 조준 등), 스폰 위치가 owner의 어느 쪽인지로 보정
            if (sign == 0f && playerAbility != null)
                sign = Mathf.Sign(spawnPos.x - playerAbility.transform.position.x);

            // 3) 그래도 0이면 기본 우측(+1)
            if (sign == 0f) sign = 1f;

            hoof.SetLateralSign(sign);
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