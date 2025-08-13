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
            dx = direction.x, dy = direction.y, dz = direction.z
        };

        return Prefix + JsonUtility.ToJson(msg);
    }
}