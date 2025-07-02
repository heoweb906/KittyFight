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

    [Header("��ų ������ ����Ʈ")]
    public List<GameObject> skillPrefabs;
    public List<SkillCard_SO> skillCards;

    public void EquipSkillByCard(SkillCard_SO card)
    {
        if (card == null)
        {
            Debug.LogWarning("[SkillWorker] SkillCard_SO�� null�Դϴ�.");
            return;
        }

        int idx = card.skillIndex;
        if (idx < 0 || idx >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillWorker] skillIndex({idx})�� skillPrefabs ������ ������ϴ�.");
            return;
        }

        GameObject objSkill = Instantiate(skillPrefabs[idx]);
        Skill skillInstance = objSkill.GetComponent<Skill>();
        if (skillInstance == null)
        {
            Debug.LogError($"[SkillWorker] �Ҵ�� �����տ� Skill ������Ʈ�� �����ϴ�. Index: {idx}");
            Destroy(objSkill);
            return;
        }

        skillInstance.SetNewBasicValue(playerAbility, this);

        SkillSlotType slotToUse =
            !mySkills.ContainsKey(SkillSlotType.Q) || mySkills[SkillSlotType.Q] == null
                ? SkillSlotType.Q
                : SkillSlotType.E;

        mySkills[slotToUse] = skillInstance;
        Debug.Log($"[SkillWorker] '{card.sSkillName}'��(��) ���� {slotToUse}�� �����߽��ϴ�.");
    }






    public void UseSkill(SkillSlotType slot)
    {
        if (mySkills.TryGetValue(slot, out Skill skill) && skill != null) skill.Activate();
        else Debug.Log($"[SkillWorker] ���� {slot}�� ��ų�� �����ϴ�.");
        return;
    }
}
