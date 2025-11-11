using UnityEngine;

public class SK_MysteryFruit : Skill
{
    [Header("HP 변화 범위(정수)")]
    [SerializeField] private int minDelta = -20;
    [SerializeField] private int maxDelta = 30;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
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

        P2PMessageSender.SendMessage(
            DamageMessageBuilder.Build(pn, newHP, null)
        );
    }
}