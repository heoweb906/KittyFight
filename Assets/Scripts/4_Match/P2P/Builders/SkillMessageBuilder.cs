using UnityEngine;

public static class SkillMessageBuilder
{
    public const string Prefix = "[SKILL]";

    public static string Build(Vector3 origin, Vector3 direction, SkillType type, int casterPlayerNumber)
    {
        var msg = new SkillExecuteMessage
        {
            player = casterPlayerNumber,
            skillType = (int)type,

            ox = origin.x, oy = origin.y, oz = origin.z,
            dx = direction.x, dy = direction.y, dz = direction.z,

            isStateOnly = false,
            randomPickIndex = 0
        };

        return Prefix + JsonUtility.ToJson(msg);
    }

    public static string BuildRandomDrawState(SkillType slot, int casterPlayerNumber, int pickIndex)
    {
        var msg = new SkillExecuteMessage
        {
            player = casterPlayerNumber,
            skillType = (int)slot,

            ox = 0f, oy = 0f, oz = 0f,
            dx = 0f, dy = 0f, dz = 0f,

            isStateOnly = true,
            randomPickIndex = pickIndex
        };

        return Prefix + JsonUtility.ToJson(msg);
    }
}