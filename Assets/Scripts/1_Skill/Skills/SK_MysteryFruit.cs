using UnityEngine;
using System.Collections;

public class SK_MysteryFruit : Skill
{
    [Header("HP 변화 범위(정수)")]
    [SerializeField] private int minDelta = -20;
    [SerializeField] private int maxDelta = 30;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;


    public override void Execute(Vector3 origin, Vector3 direction)
    {
        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 3);
        StartCoroutine(ResetAttackAnimState());


        if (effectPrefab == null) return;
        Instantiate(effectPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));


        int pn = playerAbility != null ? playerAbility.playerNumber : 0;
        if (pn != MatchResultStore.myPlayerNumber) return;

        var ph = (playerAbility ? playerAbility.GetComponent<PlayerHealth>() : null)
                 ?? GetComponent<PlayerHealth>();
        if (!ph) return;
        int delta = Random.Range(minDelta, maxDelta + 1);

        int cur = ph.CurrentHP;
        int max = ph.MaxHP;
        int newHP = Mathf.Clamp(cur + delta, 0, max);

        Debug.Log(newHP);
        ph.RemoteSetHP(newHP);


        playerAbility.PlaySFX(sfxClip);


        P2PMessageSender.SendMessage(
            DamageMessageBuilder.Build(pn, newHP, 0, null)
        );

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
    }

    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}