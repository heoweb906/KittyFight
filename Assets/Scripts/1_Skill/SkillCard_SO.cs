using UnityEngine;
using UnityEngine.UI;

public enum CardAnimationType
{
    Number_5,
    Number_15,
    Number_22,
    Number_123,
}

[System.Serializable]
public class CardillustrationPivot
{
    public Sprite sprite_Cardillustration;
    [Header("위치 조정")]
    public RectOffset margins = new RectOffset(); 
}
[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard_SO : ScriptableObject
{
    public string sSkillName;
    public int iAnimalNum;
    public int iSkillIndex;
    [Space(30)]
    [Header("스킬 기본 정보")]
    public CardillustrationPivot[] cardillustrationPivots;
    public Sprite sprite_Frame;
    public Sprite sprite_SkillName;
    public Sprite sprite_Keyword;
    public Sprite sprite_BorderLine_Left;
    public Sprite sprite_BorderLine_Right;
    [Space(10)]
    [Header("애니메이션 설정")]
    public CardAnimationType animationType = CardAnimationType.Number_5;
    [TextArea(5, 20)]
    public string description;
}