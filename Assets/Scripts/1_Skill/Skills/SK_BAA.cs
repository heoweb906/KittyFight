using UnityEngine;
using System.Collections;

public class SK_BAA : Skill
{
    [SerializeField] private float stunDuration = 2f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private GameObject stunEffectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.identity,
            playerAbility.gameObject.transform
        );

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);
        StartCoroutine(ResetAttackAnimState());

        var gm = FindObjectOfType<GameManager>();
        if (!gm) return;

        int selfNum = playerAbility.playerNumber;

        PlayerAbility enemy = null;

        if (gm.playerAbility_1 && gm.playerAbility_2)
        {
            enemy = (gm.playerAbility_1.playerNumber == selfNum)
                ? gm.playerAbility_2
                : gm.playerAbility_1;
        }

        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }


        ApplyStun(playerAbility.gameObject, stunDuration, false);
        if (enemy != null)
            ApplyStun(enemy.gameObject, stunDuration, true);
    }

    private void ApplyStun(GameObject target, float duration, bool playShockAnim)
    {
        if (!target) return;

        var stun = target.GetComponent<StunStatus>();
        if (!stun) stun = target.AddComponent<StunStatus>();
        stun.ApplyStun(duration, playShockAnim);

        Instantiate(
            stunEffectPrefab,
            target.transform.position,
            Quaternion.identity,
            target.transform
        );
    }
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}