using UnityEngine;
public static class SkillSelectBuilder
{
    public static string Build(int iPlayerNum, string sSkillName, Vector2 cardPos, SkillCard_SO _skillCard_SO)
    {
        Model_SkillSelect msg = new Model_SkillSelect
        {
            iPlayer = iPlayerNum,
            sSkillCardName = sSkillName,
            cardPosition = cardPos,
            skillCard_SO = _skillCard_SO
        };
        return "[SKILL_SELECT]" + JsonUtility.ToJson(msg);
    }
}