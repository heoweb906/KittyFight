using UnityEngine;

public static class PassiveMessageBuilder
{
    public const string Prefix = "[PASSIVE]";

    public static string Build(
        int casterPlayerNumber,
        int passiveId,
        PassiveProcType procType,
        Vector3 pos,
        Vector3 dir,
        int i0 = 0,
        float f0 = 0f,
        int targetPlayer = 0
    )
    {
        var msg = new PassiveProcMessage
        {
            player = casterPlayerNumber,
            passiveId = passiveId,
            procType = (int)procType,

            px = pos.x,
            py = pos.y,
            pz = pos.z,
            dx = dir.x,
            dy = dir.y,
            dz = dir.z,

            i0 = i0,
            f0 = f0,
            targetPlayer = targetPlayer
        };

        return Prefix + JsonUtility.ToJson(msg);
    }
}