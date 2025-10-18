using UnityEngine;

public static class DamageMessageBuilder
{
    public static string Build(int targetPlayer, int hp)
    {
        return Build(targetPlayer, hp, null);
    }

    public static string Build(int targetPlayer, int hp, Vector3? sourceWorldPos)
    {
        var msg = new DamageMessage
        {
            targetPlayer = targetPlayer,
            hp = hp,
            hasSource = sourceWorldPos.HasValue,
            sourceWorldPos = sourceWorldPos ?? Vector3.zero
        };
        return "[DMG]" + JsonUtility.ToJson(msg);
    }
}