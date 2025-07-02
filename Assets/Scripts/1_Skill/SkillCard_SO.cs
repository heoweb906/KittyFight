using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard_SO : ScriptableObject
{
    public int skillIndex;

    [Header("스킬 기본 정보")]
    public Image image_BackGround;
    public Image image_SkillIcon;
    public string sSkillName;
    public string sSkillDescription;    // 짧은 설명 (ex. "깃털을 던집니다!!!")      
    
    [TextArea(5, 20)]
    public string description;          // 긴 설명 (ex. "공격 방향으로 날아가는 날카로운 깃털을 5개 던집니다")     


}