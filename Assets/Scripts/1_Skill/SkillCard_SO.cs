using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkillCard", menuName = "Skill System/Skill Card")]
public class SkillCard_SO : ScriptableObject
{
    public string sSkillName;
    public int iAnimalNum;
    public int iSkillIndex;
    

    [Space(30)]
    [Header("스킬 기본 정보")]
    public Sprite sprite_Cardillustration;
    public Sprite sprite_CardBorderLine;
    public Sprite sprite_CardTitle;
    public Sprite sprite_CardKeyWord;

  
    [TextArea(5, 20)]
    public string description;          // 긴 설명 (ex. "공격 방향으로 날아가는 날카로운 깃털을 5개 던집니다")     


}