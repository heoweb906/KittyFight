using UnityEngine;

public static class SkillSelectBuilder
{
    public static string Build(int iPlayerNum, string sSkillName)
    {
        Model_SkillSelect msg = new Model_SkillSelect
        {
            iPlayer = iPlayerNum,
            sSkillCardName = sSkillName
        };

        return "[SKILL_SELECT]" + JsonUtility.ToJson(msg);
    }

}



