using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard : ScriptableObject
{
    

    [Header("��ų �⺻ ����")]
    public string skillName;
    public int skillIndex;

    [TextArea(5, 20)]
    public string description; // <color=#FF0000>�̷� ���� ���� �±� ���� ����


    // [Header("")]



}