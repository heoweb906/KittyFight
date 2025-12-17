using System.Collections;
using UnityEngine;

public class SK_HopCrash : Skill
{
    private void Awake()
    {
        coolTime = 9.0f;
        aimRange = 0.1f;
        anim = playerAbility.GetComponentInChildren<Animator>();
    }

    [Header("낙하 제어")]
    [SerializeField] private float diveSpeed = 26f;
    [SerializeField] private bool lockHorizontal = true; // 낙하 중 X 속도 0으로 고정

    [Header("착지 피해")]
    [SerializeField] private float baseDamage = 45f;
    [SerializeField] private float damagePerMeter = 10f;  // 낙하 거리 1m당 추가 피해

    [Header("카메라 연출")]
    [SerializeField] private float shakeStrength = 0.12f;
    [SerializeField] private float shakeDuration = 0.25f;

    [SerializeField] private float attackAnimDuration = 0.5f;
    private Animator anim;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity || playerAbility == null) return;

        GameObject owner = playerAbility.gameObject;
        var anim = owner.GetComponentInChildren<Animator>();
        if (!anim) return;

        // 공중에서만 사용 가능
        bool isGround = anim.GetBool("isGround");
        if (isGround) return;

        StartCoroutine(Co_HopCrash(owner, direction));
    }

    private IEnumerator Co_HopCrash(GameObject owner, Vector3 direction)
    {
        var rb = owner.GetComponent<Rigidbody>();
        var ph = owner.GetComponent<PlayerHealth>();
        var anim = owner.GetComponentInChildren<Animator>();
        if (!rb || !anim || !ph) yield break;

        float startY = playerAbility.gameObject.transform.position.y;
        int prevHP = GetComponent<PlayerHealth>()?.CurrentHP ?? 0;

        // 바닥에 닿거나 피격으로 HP가 감소할 때까지
        while (true)
        {
            // 피격 중단
            int curHP = ph.CurrentHP;
            if (curHP < prevHP) yield break;
            prevHP = curHP;

            // 빠른 하강(수직 속도 강제)
            Vector3 v = rb.velocity;
            v.y = -Mathf.Abs(diveSpeed);
            if (lockHorizontal) v.x = 0f;
            v.z = 0f;
            rb.velocity = v;

            if (anim.GetBool("isGround")) break;

            yield return new WaitForFixedUpdate();
        }

        float endY = playerAbility.gameObject.transform.position.y;
        float fallDist = Mathf.Max(0f, startY - endY);
        float dmgF = baseDamage + fallDist * damagePerMeter;
        int dmg = Mathf.Max(1, Mathf.RoundToInt(dmgF));

        Quaternion rot = Quaternion.identity;
        GameObject hitbox = Instantiate(objSkillEntity, playerAbility.gameObject.transform.position, rot);

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);
        StartCoroutine(ResetAttackAnimState());

        var abBase = hitbox.GetComponent<AB_HitboxBase>();
        if (abBase != null) abBase.Init(playerAbility);

        var hop = hitbox.GetComponent<AB_HopCrash>();
        if (hop != null) hop.damage = dmg;


        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeStrength, shakeDuration, direction);

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