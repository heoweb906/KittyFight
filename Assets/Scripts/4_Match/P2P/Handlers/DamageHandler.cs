using UnityEngine;

public class DamageHandler : IP2PMessageHandler
{
    readonly PlayerHealth opponentHp;
    readonly int myPlayerNumber;

    public DamageHandler(PlayerHealth opponent, int myNumber)
    {
        opponentHp = opponent;
        myPlayerNumber = myNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[DMG]");

    public void Handle(string msg)
    {
        var d = JsonUtility.FromJson<DamageMessage>(msg.Substring(5));
        if (d.targetPlayer == myPlayerNumber) return;
        opponentHp.RemoteSetHP(d.hp);
    }
}