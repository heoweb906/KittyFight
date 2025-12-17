using UnityEngine;
using System.Collections;

public class SK_WyrmfireRay : Skill
{
    [SerializeField] private LayerMask obstacleMask;

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
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());

        Vector3 start = playerAbility.transform.position;
        Vector3 dir = new Vector3(direction.x, direction.y, 0f);

        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        float maxDist = aimRange > 0f ? aimRange : 6.0f;
        float hitDist = maxDist;

        RaycastHit hit;
        if (obstacleMask.value != 0 &&
            Physics.Raycast(start, dir, out hit, maxDist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            hitDist = hit.distance;
        }

        Vector3 center = start + dir * (hitDist * 0.5f);
        center.z = start.z;

        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angleZ);

        var go = Instantiate(objSkillEntity, center, rot);

        var t = go.transform;
        var s = t.localScale;
        s.x = hitDist;
        s.y = 0.05f;
        t.localScale = s;

        var ab = go.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
    }

    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}