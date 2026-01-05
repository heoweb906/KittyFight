using UnityEngine;
using System.Collections;

public class SK_WyrmfireRay : Skill
{
    [SerializeField] private LayerMask obstacleMask;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;
        if (!objSkillEntity) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());

        Vector3 start = playerAbility.transform.position;
        Vector3 dir = new Vector3(direction.x, direction.y, 0f);

        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        // 사이즈 조절 로직 삭제로 인해 hitDist 계산 및 center 계산 불필요
        // 회전값만 계산
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angleZ);

        // 생성 위치를 center(중간 지점)가 아닌 start(시작점)로 변경
        var go = Instantiate(objSkillEntity, start, rot);

        playerAbility.PlaySFX(sfxClip);

        var ab = go.GetComponentInChildren<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

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