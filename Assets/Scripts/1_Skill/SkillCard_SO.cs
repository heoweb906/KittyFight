using UnityEngine;
using UnityEngine.UI;

public enum CardAnimationType
{
    Number_1,
    Number_3,
    Number_4,
    Number_5,
    Number_6,
    Number_7,
    Number_8,
    Number_9,
    Number_10,
    Number_11,
    Number_12,
    Number_13,
    Number_14,
    Number_15,
    Number_16,
    Number_17,
    Number_18,
    Number_19,
    Number_20,
    Number_21,
    Number_22,
    Number_23,
    Number_24,
    Number_25,
    Number_26,
    Number_27,
    Number_28,
    Number_101,
    Number_103,
    Number_104,
    Number_105,
    Number_106,
    Number_107,
    Number_108,
    Number_109,
    Number_110,
    Number_112,
    Number_113,
    Number_114,
    Number_116,
    Number_118,
    Number_119,
    Number_120,
    Number_121,
    Number_122,
    Number_123,
    Number_124,
    Number_125,
    Number_127,
    Number_130,
    Number_131,
    Number_132,
    Number_133,
    Number_134,
    Number_135,
    Number_137,
    Number_138
}

public enum CardType
{
    Active,
    Passive
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
    public CardType cardType = CardType.Active;
    public string sSkillName;
    public int iAnimalNum;
    public int iSkillIndex;

    [Space(30)]
    [Header("스킬 기본 정보")]
    public Sprite sprite_Icon;
    [Header("스킬 기본 정보")]
    public CardillustrationPivot[] cardillustrationPivots;
    public Sprite sprite_Frame;
    public Sprite sprite_Simbol;
    public Sprite sprite_SkillName;
    public Sprite sprite_Keyword;
    public Sprite sprite_BorderLine_Left;
    public Sprite sprite_BorderLine_Right;

    public CardAnimationType AnimationType;
    
    [TextArea(5, 20)]
    public string description;

    // iSkillIndex가 변경될 때 자동으로 animationType 설정
    private void OnValidate()
    {
        SetAnimationTypeFromSkillIndex();
    }

    private void SetAnimationTypeFromSkillIndex()
    {
        string enumName = $"Number_{iSkillIndex}";
        if (System.Enum.TryParse(enumName, out CardAnimationType result))
        {
            AnimationType = result;
        }
        else
        {
            // 해당하는 enum이 없으면 기본값으로 설정
            AnimationType = CardAnimationType.Number_1;
        }
    }
}