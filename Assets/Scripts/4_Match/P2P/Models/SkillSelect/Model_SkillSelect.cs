using UnityEngine;

[System.Serializable]
public class Model_SkillSelect
{
    public int iPlayer;
    public int iSkillIndex;  // skillCard_SO 대신 인덱스만 전송
    public Vector2 cardPosition;
    public SkillCard_SO skillCard_SO;

    public bool bIsRat;
    public int iRandomSkillIndex; 
}