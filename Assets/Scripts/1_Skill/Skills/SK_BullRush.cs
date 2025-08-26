using UnityEngine;
using System.Collections;

/// <summary>
/// ���콺 �������� ���ϰ� �����ϴ� ��ų.
/// - �̵��� ���� �����ڸ� ����(��Ʈ��ũ ��ġ ����ȭ�� ����)
/// - ���� ��Ʈ�ڽ��� ������ �÷��̾ ����(���� Ŭ���̾�Ʈ ��� ����)
/// </summary>
[RequireComponent(typeof(PlayerAbility))]
public class SK_BullRush : Skill
{
    [Header("Rush �̵�")]
    [Min(0.01f)] public float dashDuration = 0.12f;
    // [Min(0.01f)] public float aimRange = 3.5f;
    public Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);
    public LayerMask obstacleMask;
    public bool disableGravityDuringDash = true;

    [Header("��Ʈ�ڽ�")]
    public float hitboxLifetimePadding = 0.02f;

    private Rigidbody rb;

    private void Awake()
    {
        coolTime = 4.0f;
        if (obstacleMask == 0) obstacleMask = LayerMask.GetMask("Default", "Ground");
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity || playerAbility == null) return;

        // ���� ��Ʈ�ڽ� ����(���� ���): �÷��̾� ��ġ�� ���̰� ���� ����
        var spawnPos = playerAbility.transform.position;
        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var hb = Instantiate(objSkillEntity, spawnPos, rot);

        var abBase = hb.GetComponent<AB_HitboxBase>();
        if (abBase != null) abBase.Init(playerAbility);

        var bull = hb.GetComponent<AB_BullRush>();
        if (bull != null) bull.SetRushDirection(direction);

        // �÷��̾ �����ؼ� �Բ� �̵�
        hb.transform.SetParent(playerAbility.transform, true);
        // �����ֱ�: �뽬 �ð� + �ణ�� �е� �� �ı�
        Destroy(hb, dashDuration + hitboxLifetimePadding);

        // ���� �̵��� ���� �����ڸ� ����
        if (playerAbility.playerNumber != MatchResultStore.myPlayerNumber) return;

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
    }
}