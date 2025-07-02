using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard_SO : ScriptableObject
{
    public int skillIndex;

    [Header("��ų �⺻ ����")]
    public Image image_BackGround;
    public Image image_SkillIcon;
    public string sSkillName;
    public string sSkillDescription;    // ª�� ���� (ex. "������ �����ϴ�!!!")      
    
    [TextArea(5, 20)]
    public string description;          // �� ���� (ex. "���� �������� ���ư��� ��ī�ο� ������ 5�� �����ϴ�")     


}