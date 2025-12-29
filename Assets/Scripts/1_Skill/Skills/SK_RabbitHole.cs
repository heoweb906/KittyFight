using System.Collections;
using UnityEngine;

public class SK_RabbitHole : Skill
{
    [SerializeField] private float castDuration = 0.5f;
    [SerializeField] private float checkRadius = 0.4f;
    [SerializeField] private LayerMask blockedMask;

    private bool isCasting = false;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public bool CanTeleportPosition(Vector3 currentPos, Vector3 targetPos)
    {
        if ((targetPos - currentPos).sqrMagnitude < 0.01f)
            return false;

        if (blockedMask.value == 0)
            return true;

        Collider[] hits = Physics.OverlapSphere(
            targetPos,
            checkRadius,
            blockedMask,
            QueryTriggerInteraction.Ignore
        );

        return hits.Length == 0;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (isCasting) return;
        if (!playerAbility) return;

        GameObject owner = playerAbility.gameObject;
        Vector3 currentPos = owner.transform.position;

        Vector3 offset = new Vector3(direction.x, direction.y, 0f);
        if (offset.sqrMagnitude < 0.0001f) return;

        Vector3 targetPos = currentPos + offset;
        targetPos.z = currentPos.z;

        var gm = FindObjectOfType<GameManager>();
        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }

        StartCoroutine(Co_CastAndTeleport(owner, targetPos));
    }



    private IEnumerator Co_CastAndTeleport(GameObject owner, Vector3 targetPos)
    {
        isCasting = true;

        var rb = owner.GetComponent<Rigidbody>();
        var movement = owner.GetComponent<PlayerMovement>();

        movement?.AttachToPlatform(null);

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);

        if (rb != null)
            rb.velocity = Vector3.zero;

        float elapsed = 0f;
        float dur = Mathf.Max(0.0001f, castDuration);

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!owner)
        {
            isCasting = false;
            yield break;
        }

        anim.SetBool("isAttack", false);

        Vector3 currentPos = owner.transform.position;
        if (!CanTeleportPosition(currentPos, targetPos))
        {
            isCasting = false;
            yield break;
        }

        Vector3 finalPos = targetPos;
        finalPos.z = currentPos.z;

        if (rb != null)
            rb.position = finalPos;
        else
            owner.transform.position = finalPos;

        Instantiate(
            effectPrefab,
            currentPos,
            Quaternion.Euler(-90, 0, 0)
        );

        Instantiate(
            effectPrefab,
            finalPos,
            Quaternion.Euler(-90, 0, 0)
        );

        isCasting = false;
    }
}