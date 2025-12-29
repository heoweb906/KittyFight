using UnityEngine;
using System.Collections;
public class SK_PigOut : Skill
{
    [Header("HP 회복")]
    [SerializeField] private int healAmount = 40;

    [Header("슬로우")]
    [SerializeField] private float slowDuration = 3f;
    [SerializeField, Range(0f, 100f)]
    private float slowPercent = 40f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);
        StartCoroutine(ResetAttackAnimState());

        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.Euler(-90f, 0, 0)
        );

        int pn = playerAbility.playerNumber;
        if (pn != MatchResultStore.myPlayerNumber) return;

        var ph = playerAbility.GetComponent<PlayerHealth>() ?? GetComponent<PlayerHealth>();
        if (!ph) return;

        int cur = ph.CurrentHP;
        int max = ph.MaxHP;
        int newHP = Mathf.Clamp(cur + healAmount, 0, max);

        ph.RemoteSetHP(newHP);

        P2PMessageSender.SendMessage(
            DamageMessageBuilder.Build(pn, newHP, 0, null)
        );

        var gm = FindObjectOfType<GameManager>();
        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }

        ApplySelfSlow();
    }

    private void ApplySelfSlow()
    {
        GameObject target = playerAbility ? playerAbility.gameObject : gameObject;
        if (!target) return;

        var slow = target.GetComponent<SlowStatus>();
        if (!slow) slow = target.AddComponent<SlowStatus>();

        slow.ApplySlowPercent(slowPercent, slowDuration);
    }
    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}