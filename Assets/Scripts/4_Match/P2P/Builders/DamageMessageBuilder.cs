using UnityEngine;

public static class DamageMessageBuilder
{
    public static string Build(int targetPlayer, int hp)
    {
        var msg = new DamageMessage { targetPlayer = targetPlayer, hp = hp };
        return "[DMG]" + JsonUtility.ToJson(msg);
    }
}