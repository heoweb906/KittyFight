using System;
using UnityEngine;

public class DamageHandler : IP2PMessageHandler
{
    private const string Prefix = "[DMG]";

    readonly PlayerHealth opponentHp;
    readonly PlayerHealth myHp;
    readonly int myPlayerNumber;

    public DamageHandler(PlayerHealth opponent, PlayerHealth mine, int myNumber)
    {
        opponentHp = opponent;
        myHp = mine;
        myPlayerNumber = myNumber;
    }

    public bool CanHandle(string msg) => !string.IsNullOrEmpty(msg) && msg.StartsWith(Prefix);

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;
        if (string.IsNullOrEmpty(msg)) return;

        if (!msg.StartsWith(Prefix)) return;
        if (msg.Length <= Prefix.Length) return;

        string json = msg.Substring(Prefix.Length);
        if (string.IsNullOrWhiteSpace(json)) return;

        DamageMessage d;
        try
        {
            d = JsonUtility.FromJson<DamageMessage>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][DMG] JSON parse failed. msgLen={msg.Length} err={e.Message}");
            return;
        }

        if (d.targetPlayer == myPlayerNumber) return;

        if (opponentHp == null)
        {
            Debug.LogWarning("[P2P][DMG] opponentHp is null. Skip applying damage.");
            return;
        }

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
            if (ability?.events != null)
            {
                ability.events.EmitDealtDamage();
            }
        }
    }
}