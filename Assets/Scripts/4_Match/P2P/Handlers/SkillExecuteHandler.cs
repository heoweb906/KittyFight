using System;
using UnityEngine;

public class SkillExecuteHandler : IP2PMessageHandler
{
    private readonly PlayerAbility opponentAbility;
    private readonly int myPlayerNumber;

    public SkillExecuteHandler(PlayerAbility opponentAbility, int myPlayerNumber)
    {
        this.opponentAbility = opponentAbility;
        this.myPlayerNumber = myPlayerNumber;
    }

    public bool CanHandle(string msg)
    {
        var prefix = SkillMessageBuilder.Prefix;
        return !string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(msg) && msg.StartsWith(prefix);
    }

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;
        if (string.IsNullOrEmpty(msg)) return;
        if (opponentAbility == null) return;

        var prefix = SkillMessageBuilder.Prefix;
        if (string.IsNullOrEmpty(prefix)) return;

        if (!msg.StartsWith(prefix)) return;
        if (msg.Length <= prefix.Length) return;

        var json = msg.Substring(prefix.Length);
        if (string.IsNullOrWhiteSpace(json)) return;

        SkillExecuteMessage data;
        try
        {
            data = JsonUtility.FromJson<SkillExecuteMessage>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][SKILL] JSON parse failed. msgLen={msg.Length} err={e.Message}");
            return;
        }

        if (data == null) return;

        if (data.player == myPlayerNumber) return;

        if (!Enum.IsDefined(typeof(SkillType), data.skillType))
        {
            Debug.LogWarning($"[P2P][SKILL] Invalid skillType={data.skillType}. Skip.");
            return;
        }

        var type = (SkillType)data.skillType;

        if (data.isStateOnly)
        {
            var s = opponentAbility.GetSkill(type);
            if (s is SK_RandomDraw rd)
            {
                rd.ApplyRemotePick(data.randomPickIndex);
            }
            return;
        }

        Vector3 origin = new Vector3(data.ox, data.oy, data.oz);
        Vector3 dir = new Vector3(data.dx, data.dy, data.dz);

        try
        {
            opponentAbility.TryExecuteSkill(type, origin, dir, force: true);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][SKILL] TryExecuteSkill crashed. type={type} err={e.Message}");
        }
    }
}