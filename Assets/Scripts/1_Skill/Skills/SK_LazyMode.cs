using System.Collections;
using UnityEngine;

public class SK_LazyMode : Skill
{
    [Header("LazyMode 설정")]
    [SerializeField] private float lazyDuration = 1.5f;

    private bool isRunning = false;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (isRunning) return;
        if (!playerAbility) return;

        GameObject owner = playerAbility.gameObject;
        if (!owner) return;

        var health = owner.GetComponent<PlayerHealth>();
        if (!health) return;

        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            health.SetSkillInvincible(lazyDuration);
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

        StartCoroutine(Co_LazyMode(owner));
    }

    private IEnumerator Co_LazyMode(GameObject owner)
    {
        isRunning = true;

        var controller = owner.GetComponent<PlayerController>();
        var movement = owner.GetComponent<PlayerMovement>();
        var rb = owner.GetComponent<Rigidbody>();

        GameObject barrierInstance = null;
        if (objSkillEntity != null)
        {
            barrierInstance = Object.Instantiate(
                objSkillEntity,
                owner.transform.position,
                Quaternion.identity
            );
            barrierInstance.transform.SetParent(owner.transform, worldPositionStays: true);
        }

        if (rb != null)
        {
            var v = rb.velocity;
            v.x = 0f;
            v.z = 0f;
            rb.velocity = v;
        }

        bool isMine = playerAbility.playerNumber == MatchResultStore.myPlayerNumber;
        if (isMine)
        {
            if (controller) controller.enabled = false;
            if (movement) movement.enabled = false;
        }

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 5);

        yield return new WaitForSeconds(lazyDuration);

        if (isMine)
        {
            if (controller) controller.enabled = true;
            if (movement) movement.enabled = true;
        }

        if (barrierInstance != null)
        {
            Object.Destroy(barrierInstance);
        }

        anim.SetTrigger("Attack");
        anim.SetInteger("AttackType", 6);
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
        isRunning = false;
    }
}