using System.Collections.Generic;
using UnityEngine;

public enum SkillSlotType
{
    Q,
    E
}

public class SkillWorker : MonoBehaviour
{
    public PlayerAbility playerAbility;
    private Dictionary<SkillSlotType, Skill> mySkills = new Dictionary<SkillSlotType, Skill>();

    [Header("스킬 프리팹 리스트")]
    public List<GameObject> skillPrefabs;
    public List<SkillCard_SO> skillCards;

    public void EquipSkillByCard(SkillCard_SO card)
    {
        if (card == null)
        {
            Debug.LogWarning("[SkillWorker] SkillCard_SO가 null입니다.");
            return;
        }

        int idx = card.skillIndex;
        if (idx < 0 || idx >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillWorker] skillIndex({idx})가 skillPrefabs 범위를 벗어났습니다.");
            return;
        }

        GameObject objSkill = Instantiate(skillPrefabs[idx]);
        Skill skillInstance = objSkill.GetComponent<Skill>();
        if (skillInstance == null)
        {
            Debug.LogError($"[SkillWorker] 할당된 프리팹에 Skill 컴포넌트가 없습니다. Index: {idx}");
            Destroy(objSkill);
            return;
        }

        skillInstance.SetNewBasicValue(playerAbility, this);

        SkillSlotType slotToUse =
            !mySkills.ContainsKey(SkillSlotType.Q) || mySkills[SkillSlotType.Q] == null
                ? SkillSlotType.Q
                : SkillSlotType.E;

        mySkills[slotToUse] = skillInstance;
        Debug.Log($"[SkillWorker] '{card.sSkillName}'을(를) 슬롯 {slotToUse}에 장착했습니다.");
    }






    public void UseSkill(SkillSlotType slot)
    {
        if (mySkills.TryGetValue(slot, out Skill skill) && skill != null) skill.Activate();
        else Debug.Log($"[SkillWorker] 슬롯 {slot}에 스킬이 없습니다.");
        return;
    }
}
