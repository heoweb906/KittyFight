using UnityEngine;

[System.Serializable]

public class Model_SkillSelect
{
    public int iPlayer;
    public string sSkillCardName;

    public Vector2 cardPosition;  // 추가
    public SkillCard_SO skillCard_SO;   // 추가
}