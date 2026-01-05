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


        // 러시 히트박스 스폰(양쪽 모두): 플레이어 위치에 붙이고 방향 전달
        var spawnPos = playerAbility.transform.position;
        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var hb = Instantiate(objSkillEntity, spawnPos, rot);

        anim.SetBool("isDash", true);
        anim.SetTrigger("Dash");

        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.Euler(0, rot.eulerAngles.y + 180f, 0)
        );
        playerAbility.PlaySFX(sfxClip);

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
        var dirNorm = new Vector3(direction.x, 0f, direction.z).normalized;
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
}