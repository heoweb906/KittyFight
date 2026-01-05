using UnityEngine;
using System.Collections;

public class SK_BullRush : Skill
{
    [Header("Rush 이동")]
    [Min(0.01f)] public float dashDuration = 0.12f;
    public Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);
    public LayerMask obstacleMask;
    public bool disableGravityDuringDash = true;

    [Header("히트박스")]
    public float hitboxLifetimePadding = 0.02f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    private Rigidbody rb;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    private void Awake()
    {
        coolTime = 4.0f;
        if (obstacleMask == 0) obstacleMask = LayerMask.GetMask("Default", "Ground");
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity || playerAbility == null) return;

        direction = GetAutoDirectionToOpponent(direction);
        if (direction.sqrMagnitude < 0.0001f) return;

        // 러시 히트박스 스폰(양쪽 모두): 플레이어 위치에 붙이고 방향 전달
        var spawnPos = playerAbility.transform.position;
        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var hb = Instantiate(objSkillEntity, spawnPos, rot);

        anim.SetBool("isDash", true);
        anim.SetTrigger("Dash");

        if (effectPrefab != null)
        {
            Vector2 d2 = new Vector2(direction.x, direction.y);
            if (d2.sqrMagnitude < 0.0001f) d2 = Vector2.right;
            d2.Normalize();
            float xAngle = Mathf.Atan2(d2.y, d2.x) * Mathf.Rad2Deg;
            Quaternion fxRot = Quaternion.Euler(xAngle, -90f, 0f);

            Instantiate(effectPrefab, playerAbility.transform.position, fxRot);
        }

        var abBase = hb.GetComponent<AB_HitboxBase>();
        if (abBase != null) abBase.Init(playerAbility);

        var bull = hb.GetComponent<AB_BullRush>();
        if (bull != null) bull.SetRushDirection(direction);


        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }


        // 플레이어에 부착해서 함께 이동
        hb.transform.SetParent(playerAbility.transform, true);
        // 생명주기: 대쉬 시간 + 약간의 패딩 후 파괴
        Destroy(hb, dashDuration + hitboxLifetimePadding);


        // 실제 이동은 로컬 소유자만 수행
        //if (playerAbility.playerNumber != MatchResultStore.myPlayerNumber) return;

        if (!rb)
        {
            rb = playerAbility.GetComponent<Rigidbody>();
            if (!rb) rb = GetComponentInParent<Rigidbody>();
            if (!rb) return;
        }

        var startPos = rb.position;
        var dirNorm = new Vector3(direction.x, direction.y, direction.z).normalized;
        if (dirNorm.sqrMagnitude < 0.0001f) return;
        float maxDist = aimRange;

        if (Physics.BoxCast(startPos, boxHalfExtents, dirNorm, out var hit,
                            Quaternion.identity, aimRange, obstacleMask))
            maxDist = hit.distance;

        var targetPos = startPos + dirNorm * maxDist;

        StartCoroutine(DashLerp(startPos, targetPos));
    }

    private IEnumerator DashLerp(Vector3 startPos, Vector3 targetPos)
    {
        bool origUseGravity = rb.useGravity;
        Vector3 origVel = rb.velocity;

        if (disableGravityDuringDash) rb.useGravity = false;
        rb.velocity = Vector3.zero;

        float elapsed = 0f;
        float dur = Mathf.Max(0.0001f, dashDuration);
        while (elapsed < dur)
        {
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, elapsed / dur));
            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.MovePosition(targetPos);

        if (disableGravityDuringDash) rb.useGravity = origUseGravity;
        rb.velocity = Vector3.zero;
        anim.SetBool("isDash", false);
    }

    private PlayerAbility FindOpponentAbility()
    {
        if (playerAbility == null) return null;

        var gm = FindObjectOfType<GameManager>();
        if (gm == null) return null;

        var pa1 = gm.playerAbility_1;
        var pa2 = gm.playerAbility_2;

        if (pa1 == null || pa2 == null) return null;

        if (playerAbility == pa1) return pa2;
        if (playerAbility == pa2) return pa1;

        if (playerAbility.playerNumber == 1) return pa2;
        if (playerAbility.playerNumber == 2) return pa1;

        return null;
    }

    private Vector3 GetAutoDirectionToOpponent(Vector3 fallbackDir)
    {
        var opp = FindOpponentAbility();
        if (opp == null)
            return fallbackDir;

        Vector3 dir = opp.transform.position - playerAbility.transform.position;
        dir.z = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return fallbackDir;

        return dir.normalized;
    }
    public Vector3 GetAutoDirection(Vector3 fallbackDir)
    {
        return GetAutoDirectionToOpponent(fallbackDir);
    }
}