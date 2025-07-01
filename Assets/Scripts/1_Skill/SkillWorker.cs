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
    // SkillCard_SO.skillIndex에 맞춰서 순서대로 프리팹을 할당하세요.
    public List<GameObject> skillPrefabs;
    public List<SkillCard_SO> skillCards;

    /// <summary>
    /// SkillCard_SO 정보만으로 슬롯에 스킬을 생성해 장착합니다.
    /// </summary>
    public void EquipSkillByCard(SkillSlotType slot, SkillCard_SO card)
    {
        if (card == null)
        {
            Debug.LogWarning($"[SkillWorker] SkillCard_SO가 null입니다. 슬롯: {slot}");
            return;
        }

        int idx = card.skillIndex;
        if (idx < 0 || idx >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillWorker] skillIndex({idx})가 skillPrefabs 범위를 벗어났습니다.");
            return;
        }

        // 프리팹 인스턴스화
        GameObject go = Instantiate(skillPrefabs[idx]);
        Skill skillInstance = go.GetComponent<Skill>();
        if (skillInstance == null)
        {
            Debug.LogError($"[SkillWorker] 할당된 프리팹에 Skill 컴포넌트가 없습니다. Index: {idx}");
            Destroy(go);
            return;
        }

        // Skill 초기화 (Skill 클래스에 Initialize 메서드가 구현되어 있어야 함)
        skillInstance.Initialize(playerAbility, this);

        mySkills[slot] = skillInstance;
        Debug.Log($"[SkillWorker] '{card.sSkillName}'을(를) 슬롯 {slot}에 장착했습니다.");
    }

    /// <summary>
    /// 슬롯에 장착된 스킬을 실행합니다.
    /// </summary>
    public void UseSkill(SkillSlotType slot)
    {
        if (mySkills.TryGetValue(slot, out Skill skill) && skill != null)
        {
            skill.Activate();
        }
        else
        {
            Debug.Log($"[SkillWorker] 슬롯 {slot}에 스킬이 없습니다.");
        }
    }

    /// <summary>
    /// 빈 슬롯이 있다면 Q 슬롯에, 그렇지 않으면 E 슬롯에 자동으로 장착합니다.
    /// </summary>
    public void AutoEquip(Skill skill)
    {
        SkillSlotType slotToUse =
            !mySkills.ContainsKey(SkillSlotType.Q) || mySkills[SkillSlotType.Q] == null
                ? SkillSlotType.Q
                : SkillSlotType.E;

        mySkills[slotToUse] = skill;
        Debug.Log($"[SkillWorker] '{skill.GetType().Name}'을(를) 슬롯 {slotToUse}에 자동 장착했습니다.");
    }
}