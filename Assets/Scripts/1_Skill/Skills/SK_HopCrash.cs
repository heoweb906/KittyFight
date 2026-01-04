using System.Collections;
using UnityEngine;

public class SK_HopCrash : Skill
{
    private void Awake()
    {
        coolTime = 9.0f;
        aimRange = 0.1f;
    }

    [Header("낙하 제어")]
    [SerializeField] private float diveSpeed = 26f;
    [SerializeField] private bool lockHorizontal = true; // 낙하 중 X 속도 0으로 고정

    [Header("착지 피해")]
    [SerializeField] private float baseDamage = 45f;
    [SerializeField] private float damagePerMeter = 10f;  // 낙하 거리 1m당 추가 피해

    [Header("착지 판정 (Layer 기반)")]
    [SerializeField] private LayerMask stopLayerMask; // Player | Wall | Ground
    [SerializeField] private float castRadius = 0.25f;
    [SerializeField] private float castExtra = 0.15f; // 프레임 보정 여유


    [Header("카메라 연출")]
    [SerializeField] private float shakeStrength = 0.12f;
    [SerializeField] private float shakeDuration = 0.25f;

    [SerializeField] private float attackAnimDuration = 0.5f;


    [Header("Effect")]
    public GameObject effectUse;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity || playerAbility == null) return;

        GameObject owner = playerAbility.gameObject;
        var anim = owner.GetComponentInChildren<Animator>();
        if (!anim) return;

        // 공중에서만 사용 가능
        bool isGround = anim.GetBool("isGround");
        if (isGround) return;


        playerAbility.PlaySFX(sfxClip);
        Instantiate(
       effectUse,
       playerAbility.gameObject.transform.position,
       Quaternion.identity,
       playerAbility.gameObject.transform
   );



        StartCoroutine(Co_HopCrash(owner, direction));
    }

    private IEnumerator Co_HopCrash(GameObject owner, Vector3 direction)
    {
        var rb = owner.GetComponent<Rigidbody>();
        var ph = owner.GetComponent<PlayerHealth>();
        var anim = owner.GetComponentInChildren<Animator>();
        var pj = owner.GetComponent<PlayerJump>();

        if (!rb || !anim || !ph) yield break;
        if (pj != null) pj.PushDisableWallSlide();

        float startY = playerAbility.gameObject.transform.position.y;
        int prevHP = GetComponent<PlayerHealth>()?.CurrentHP ?? 0;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);
        StartCoroutine(ResetAttackAnimState());

        // 바닥 / 벽 / 플레이어에 닿거나 피격되면 종료
        while (true)
        {
            // 피격 중단
            int curHP = ph.CurrentHP;
            if (curHP < prevHP) yield break;
            prevHP = curHP;

            // 수직 낙하 강제
            Vector3 v = rb.velocity;
            v.y = -Mathf.Abs(diveSpeed);
            if (lockHorizontal) v.x = 0f;
            v.z = 0f;
            rb.velocity = v;

            // 이번 FixedUpdate에서 내려갈 거리 + 여유
            float stepDown = Mathf.Abs(rb.velocity.y) * Time.fixedDeltaTime + castExtra;

            Vector3 castOrigin = owner.transform.position;

            // Layer 기반 착지 판정
            if (Physics.SphereCast(
                    castOrigin,
                    castRadius,
                    Vector3.down,
                    out RaycastHit hit,
                    stepDown,
                    stopLayerMask,
                    QueryTriggerInteraction.Ignore))
            {
                // 충돌 지점에 살짝 띄워서 보정
                Vector3 p = rb.position;
                p.y = hit.point.y + castRadius;
                rb.position = p;

                break;
            }

            yield return new WaitForFixedUpdate();
        }

        if (pj != null) pj.PopDisableWallSlide();

        float endY = playerAbility.gameObject.transform.position.y;
        float fallDist = Mathf.Max(0f, startY - endY);
        float dmgF = baseDamage + fallDist * damagePerMeter;
        int dmg = Mathf.Max(1, Mathf.RoundToInt(dmgF));

        Quaternion rot = Quaternion.identity;
        GameObject hitbox = Instantiate(objSkillEntity, playerAbility.gameObject.transform.position, rot);

        var abBase = hitbox.GetComponent<AB_HitboxBase>();
        if (abBase != null) abBase.Init(playerAbility);

        var hop = hitbox.GetComponent<AB_HopCrash>();
        if (hop != null) hop.damage = dmg;

        var gm = FindObjectOfType<GameManager>();
        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeStrength, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeStrength * 0.5f, shakeDuration * 0.5f, direction);
        }
        //if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        //{

        //}
    }

    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}