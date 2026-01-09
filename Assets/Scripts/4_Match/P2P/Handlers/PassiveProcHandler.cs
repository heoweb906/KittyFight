using System;
using UnityEngine;

public class PassiveProcHandler : IP2PMessageHandler
{
    private readonly PlayerAbility myAbility;
    private readonly PlayerAbility opponentAbility;
    private readonly int myPlayerNumber;

    public PassiveProcHandler(PlayerAbility myAbility, PlayerAbility opponentAbility, int myPlayerNumber)
    {
        this.myAbility = myAbility;
        this.opponentAbility = opponentAbility;
        this.myPlayerNumber = myPlayerNumber;
    }

    public bool CanHandle(string msg)
    {
        var prefix = PassiveMessageBuilder.Prefix;
        return !string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(msg) && msg.StartsWith(prefix);
    }

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;
        if (string.IsNullOrEmpty(msg)) return;

        var prefix = PassiveMessageBuilder.Prefix;
        if (string.IsNullOrEmpty(prefix)) return;

        if (!msg.StartsWith(prefix)) return;
        if (msg.Length <= prefix.Length) return;

        var json = msg.Substring(prefix.Length);
        if (string.IsNullOrWhiteSpace(json)) return;

        PassiveProcMessage data;
        try
        {
            data = JsonUtility.FromJson<PassiveProcMessage>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][PASSIVE] JSON parse failed. msgLen={msg.Length} err={e.Message}");
            return;
        }

        if (data == null) return;
        if (data.player == myPlayerNumber) return;

        if (data.passiveId == 133 || data.passiveId == 134)
        {
            if (myAbility == null) return;
            myAbility.RemoteExecutePassive(data);
            return;
        }

        if (opponentAbility == null) return;
        opponentAbility.RemoteExecutePassive(data);
    }
}