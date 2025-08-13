using UnityEngine;
public static class SkillShowBuilder
{
    public static string Build(int iPlayerNum, string sSkillName, int[] _arraySkilNumbers)
    {
        Model_SkillShow msg = new Model_SkillShow
        {
            iPlayer = iPlayerNum,
            sSkillCardName = sSkillName,
            arraySkilNumbers = _arraySkilNumbers
        };
        return "[SKILL_SHOW]" + JsonUtility.ToJson(msg);
    }
}