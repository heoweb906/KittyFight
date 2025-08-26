using UnityEngine;

public static class SkillShowBuilder
{
    public static string Build(int iPlayerNum, int[] _iArraySkillCardNum)
    {
        Model_SkillShow msg = new Model_SkillShow
        {
            iPlayer = iPlayerNum,
            iArraySkillCardNum = _iArraySkillCardNum
        };

        return "[SKILL_SHOW]" + JsonUtility.ToJson(msg);
    }

}
