using Amazon.GameLift.Model;
using System.Collections;
using UnityEngine;

public class SK_TailWhipline : Skill
{
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private float pullDuration = 0.15f;
    [SerializeField] private LayerMask hitMask;

    private bool isRunning = false;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [Header("이펙트")]
    public GameObject objEffect;




    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (isRunning) return;
        if (!playerAbility) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());

        Vector3 start = playerAbility.transform.position;
        Vector3 dir = new Vector3(direction.x, direction.y, 0f);
        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        float maxDist = aimRange;
        if (maxDist <= 0f) maxDist = 6f;

        var owner = playerAbility.gameObject;

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


        StartCoroutine(Co_TailRoutine(owner, start, dir, maxDist));
    }

    private IEnumerator Co_TailRoutine(GameObject owner, Vector3 origin, Vector3 dir, float maxDist)
    {
        isRunning = true;

        var rb = owner.GetComponent<Rigidbody>();
        var controller = owner.GetComponent<PlayerController>();
        var movement = owner.GetComponent<PlayerMovement>();
        var selfHealth = owner.GetComponent<PlayerHealth>();

        int startHP = selfHealth != null ? selfHealth.CurrentHP : 0;
        bool canceledByHit = false;

        if (rb)
        {
            var v = rb.velocity;
            v.x = 0f; v.z = 0f;
            rb.velocity = v;
        }

        bool isMine = playerAbility.playerNumber == MatchResultStore.myPlayerNumber;
        if (isMine)
        {
            if (controller) controller.enabled = false;
            if (movement) movement.enabled = false;
        }


        GameObject tail = null;
        Transform tailTr = null;

        if (objSkillEntity)
        {
            tail = Object.Instantiate(objSkillEntity, origin, Quaternion.identity);
            tailTr = tail.transform;

            var abBase = tail.GetComponent<AB_HitboxBase>();
            if (abBase != null) abBase.Init(playerAbility);

            var stretcher = tail.GetComponent<ProjectileStretcher>();
            if (stretcher != null)
            {
                stretcher.OnLaunch(owner.transform, origin);
            }
        }

        float traveled = 0f;
        Vector3 prevPos = origin;
        bool hasHit = false;
        Vector3 hitPoint = origin;

        while (traveled < maxDist)
        {
            if (!owner)
            {
                canceledByHit = true;
                break;
            }

            if (selfHealth != null && selfHealth.CurrentHP != startHP)
            {
                canceledByHit = true;
                break;
            }

            float step = projectileSpeed * Time.deltaTime;
            if (traveled + step > maxDist) step = maxDist - traveled;
            if (step <= 0f) break;

            RaycastHit hit;
            if (Physics.SphereCast(
                    prevPos,
                    collisionRadius,
                    dir,
                    out hit,
                    step,
                    hitMask,
                    QueryTriggerInteraction.Ignore))
            {
                hasHit = true;
                hitPoint = hit.point;
                hitPoint.z = origin.z;

                playerAbility.PlaySFX(sfxClip);

                if (tailTr) tailTr.position = hitPoint;

                // ▼▼▼ [수정됨 1] 적중 즉시 꼬리 오브젝트 삭제 ▼▼▼
                if (tail) Object.Destroy(tail);
                // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

                if (objEffect != null)
                {
                    GameObject instance = Instantiate(objEffect, hitPoint, Quaternion.LookRotation(hit.normal));
                    instance.transform.localScale = Vector3.one * 2f;

                    // ▼▼▼ [수정됨 2] 원본 프리팹 건드리는 위험한 코드 삭제 ▼▼▼
                    // objEffect.transform.SetParent(null); <-- 이 줄은 반드시 지워야 합니다!
                }

                Debug.DrawLine(prevPos, hitPoint, Color.red, 0.3f);
                break;
            }

            Vector3 newPos = prevPos + dir * step;
            newPos.z = origin.z;

            if (tailTr) tailTr.position = newPos;

            Debug.DrawLine(prevPos, newPos, Color.yellow, 0.1f);

            prevPos = newPos;
            traveled += step;

            yield return null;
        }

        if (canceledByHit)
        {
            // 혹시 loop 안에서 삭제 안 됐을 경우를 대비
            if (tail) Object.Destroy(tail);
            Restore(owner, rb, controller, movement);
            isRunning = false;
            yield break;
        }

        if (hasHit)
        {
            Vector3 startPos = owner.transform.position;
            Vector3 targetPos = hitPoint;
            targetPos.z = origin.z;

            float elapsed = 0f;
            float dur = Mathf.Max(0.001f, pullDuration);

            while (elapsed < dur)
            {
                if (!owner) break;

                if (selfHealth != null && selfHealth.CurrentHP != startHP)
                {
                    canceledByHit = true;
                    break;
                }

                float t = elapsed / dur;
                Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
                pos.z = origin.z;

                if (rb)
                    rb.MovePosition(pos);
                else
                    owner.transform.position = pos;

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!canceledByHit && owner)
            {
                Vector3 finalPos = targetPos;
                finalPos.z = origin.z;

                if (rb)
                    rb.MovePosition(finalPos);
                else
                    owner.transform.position = finalPos;
            }
        }

        // 마지막 안전장치 (이미 위에서 삭제했으면 무시됨)
        if (tail) Object.Destroy(tail);

        Restore(owner, rb, controller, movement);
        isRunning = false;
    }

    private void Restore(
        GameObject owner,
        Rigidbody rb,
        PlayerController controller,
        PlayerMovement movement
    )
    {
        if (!owner) return;

        if (rb)
        {
            var v = rb.velocity;
            v.x = 0f; v.z = 0f;
            rb.velocity = v;
        }

        bool isMine = playerAbility.playerNumber == MatchResultStore.myPlayerNumber;
        if (isMine)
        {
            if (controller) controller.enabled = true;
            if (movement) movement.enabled = true;
        }
    }
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}