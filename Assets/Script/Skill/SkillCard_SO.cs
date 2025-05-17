using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard_SO : ScriptableObject
{
   
    [Header("��ų �⺻ ����")]
    public Image image_BackGround;
    public Image image_SkillIcon;
    public string sSkillName;
    public string sSkillDescription;
    
    public int skillIndex;

    [TextArea(5, 20)]
    public string description; // <color=#FF0000>�̷� ���� ���� �±� ���� ����


}