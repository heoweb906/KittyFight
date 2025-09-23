using UnityEngine;
using UnityEngine.UI;

public enum CardAnimationType
{
    Number_1,
    Number_3,
    Number_5,
    Number_12,
    Number_15,
    Number_16,
    Number_20,
    Number_22,
    Number_24,
    Number_101,
    Number_103,
    Number_108,
    Number_109,
    Number_113,
    Number_123,
    Number_133,
    Number_137
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
    [Header("��ġ ����")]
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
    [Header("��ų �⺻ ����")]
    public Sprite sprite_Icon;
    [Header("��ų �⺻ ����")]
    public CardillustrationPivot[] cardillustrationPivots;
    public Sprite sprite_Frame;
    public Sprite sprite_SkillName;
    public Sprite sprite_Keyword;
    public Sprite sprite_BorderLine_Left;
    public Sprite sprite_BorderLine_Right;

    public CardAnimationType AnimationType { get; set; }
    
    [TextArea(5, 20)]
    public string description;

    // iSkillIndex�� ����� �� �ڵ����� animationType ����
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
            // �ش��ϴ� enum�� ������ �⺻������ ����
            AnimationType = CardAnimationType.Number_1;
        }
    }
}