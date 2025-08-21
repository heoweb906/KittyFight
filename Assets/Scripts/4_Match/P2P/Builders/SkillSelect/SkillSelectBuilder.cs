using UnityEngine;
public static class SkillSelectBuilder
{
    public static string Build(int iPlayerNum, string sSkillName, Vector2 cardPos)
    {
        Model_SkillSelect msg = new Model_SkillSelect
        {
            iPlayer = iPlayerNum,
            sSkillCardName = sSkillName,
            cardPosition = cardPos
        };
        return "[SKILL_SELECT]" + JsonUtility.ToJson(msg);
    }
}