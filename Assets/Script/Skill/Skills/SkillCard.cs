using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard : ScriptableObject
{
    

    [Header("스킬 기본 정보")]
    public string skillName;
    public int skillIndex;

    [TextArea(5, 20)]
    public string description; // <color=#FF0000>이런 식의 강조 태그 삽입 가능


    // [Header("")]



}