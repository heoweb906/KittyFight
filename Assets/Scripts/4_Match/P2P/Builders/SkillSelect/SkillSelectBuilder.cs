using UnityEngine;
public static class SkillSelectBuilder
{
    public static string Build(int iPlayerNum, string sSkillName, Vector2 cardPos, SkillCard_SO _skillCard_SO, bool bIsRat_, int _iRatSkillNum)
    {
        Model_SkillSelect msg = new Model_SkillSelect
        {
            iPlayer = iPlayerNum,
            sSkillCardName = sSkillName,
            cardPosition = cardPos,
            skillCard_SO = _skillCard_SO,
            bIsRat = bIsRat_,
            iRandomSkillIndex = _iRatSkillNum
        };
        return "[SKILL_SELECT]" + JsonUtility.ToJson(msg);
    }
}