using UnityEngine;
using System.Collections;

public class SK_Hound : Skill
{
    [Header("슬로우 설정")]
    [SerializeField] private float slowDuration = 3f;
    [SerializeField, Range(0f, 100f)]
    private float slowPercent = 50f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private GameObject slowEffectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());

        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.identity,
            playerAbility.gameObject.transform
        );

        playerAbility.PlaySFX(sfxClip);

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

        if (enemy != null)
        {
            ApplySlow(enemy.gameObject);
        }

        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }
    }

    private void ApplySlow(GameObject target)
    {
        if (!target) return;

        var slow = target.GetComponent<SlowStatus>();
        if (!slow) slow = target.AddComponent<SlowStatus>();

        slow.ApplySlowPercent(slowPercent, slowDuration);

        Instantiate(
            slowEffectPrefab,
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