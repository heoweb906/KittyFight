using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard_SO : ScriptableObject
{
   
    [Header("스킬 기본 정보")]
    public Image image_BackGround;
    public Image image_SkillIcon;
    public string sSkillName;
    public string sSkillDescription;
    
    public int skillIndex;

    [TextArea(5, 20)]
    public string description; // <color=#FF0000>이런 식의 강조 태그 삽입 가능


}