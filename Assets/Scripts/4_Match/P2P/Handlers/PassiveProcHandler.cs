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

    public bool CanHandle(string msg) => msg.StartsWith(PassiveMessageBuilder.Prefix);

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;

        var json = msg.Substring(PassiveMessageBuilder.Prefix.Length);
        var data = JsonUtility.FromJson<PassiveProcMessage>(json);
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