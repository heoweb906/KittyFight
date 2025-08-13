using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard_SO : ScriptableObject
{
    public string sSkillName;
    public int iAnimalNum;
    public int iSkillIndex;
    

    [Space(30)]
    [Header("��ų �⺻ ����")]
    public Sprite sprite_Cardillustration;
    public Sprite sprite_CardBorderLine;
    public Sprite sprite_CardTitle;
    public Sprite sprite_CardKeyWord;

  
    [TextArea(5, 20)]
    public string description;          // �� ���� (ex. "���� �������� ���ư��� ��ī�ο� ������ 5�� �����ϴ�")     


}