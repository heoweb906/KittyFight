using UnityEngine;

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

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

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
}