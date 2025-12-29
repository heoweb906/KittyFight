using UnityEngine;

public class DamageHandler : IP2PMessageHandler
{
    readonly PlayerHealth opponentHp;
    readonly PlayerHealth myHp;
    readonly int myPlayerNumber;

    public DamageHandler(PlayerHealth opponent, PlayerHealth mine, int myNumber)
    {
        opponentHp = opponent;
        myHp = mine;
        myPlayerNumber = myNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[DMG]");

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;

        var d = JsonUtility.FromJson<DamageMessage>(msg.Substring(5));
        if (d.targetPlayer == myPlayerNumber) return;

        if (d.maxHp > 0)
        {
            opponentHp.SetMaxHP(d.maxHp, keepCurrentRatio: false);
        }

        if (d.hasSource)
            opponentHp.RemoteSetHP(d.hp, d.sourceWorldPos);
        else
            opponentHp.RemoteSetHP(d.hp);

        if (d.attackPlayer == myPlayerNumber && myHp != null)
        {
            var ability = myHp.GetComponent<PlayerAbility>();
            if (ability != null && ability.events != null)
            {
                ability.events.EmitDealtDamage();
            }
        }
    }
}