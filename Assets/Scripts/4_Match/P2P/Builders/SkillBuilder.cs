using UnityEngine;

public static class SkillBuilder 
{
    public static string Build(string skillName, int player, string slot)
{
    Model_Skill msg = new Model_Skill
    {
        skillName = skillName,
        player = player,
        slot = slot
    };
    return "[SKILL]" + JsonUtility.ToJson(msg);
}
}
