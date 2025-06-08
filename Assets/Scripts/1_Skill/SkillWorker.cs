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
    // SkillCard_SO.skillIndex�� ���缭 ������� �������� �Ҵ��ϼ���.
    public List<GameObject> skillPrefabs;
    public List<SkillCard_SO> skillCards;

    /// <summary>
    /// SkillCard_SO ���������� ���Կ� ��ų�� ������ �����մϴ�.
    /// </summary>
    public void EquipSkillByCard(SkillSlotType slot, SkillCard_SO card)
    {
        if (card == null)
        {
            Debug.LogWarning($"[SkillWorker] SkillCard_SO�� null�Դϴ�. ����: {slot}");
            return;
        }

        int idx = card.skillIndex;
        if (idx < 0 || idx >= skillPrefabs.Count)
        {
            Debug.LogError($"[SkillWorker] skillIndex({idx})�� skillPrefabs ������ ������ϴ�.");
            return;
        }

        // ������ �ν��Ͻ�ȭ
        GameObject go = Instantiate(skillPrefabs[idx]);
        Skill skillInstance = go.GetComponent<Skill>();
        if (skillInstance == null)
        {
            Debug.LogError($"[SkillWorker] �Ҵ�� �����տ� Skill ������Ʈ�� �����ϴ�. Index: {idx}");
            Destroy(go);
            return;
        }

        // Skill �ʱ�ȭ (Skill Ŭ������ Initialize �޼��尡 �����Ǿ� �־�� ��)
        skillInstance.Initialize(playerAbility, this);

        mySkills[slot] = skillInstance;
        Debug.Log($"[SkillWorker] '{card.sSkillName}'��(��) ���� {slot}�� �����߽��ϴ�.");
    }

    /// <summary>
    /// ���Կ� ������ ��ų�� �����մϴ�.
    /// </summary>
    public void UseSkill(SkillSlotType slot)
    {
        if (mySkills.TryGetValue(slot, out Skill skill) && skill != null)
        {
            skill.Activate();
        }
        else
        {
            Debug.Log($"[SkillWorker] ���� {slot}�� ��ų�� �����ϴ�.");
        }
    }

    /// <summary>
    /// �� ������ �ִٸ� Q ���Կ�, �׷��� ������ E ���Կ� �ڵ����� �����մϴ�.
    /// </summary>
    public void AutoEquip(Skill skill)
    {
        SkillSlotType slotToUse =
            !mySkills.ContainsKey(SkillSlotType.Q) || mySkills[SkillSlotType.Q] == null
                ? SkillSlotType.Q
                : SkillSlotType.E;

        mySkills[slotToUse] = skill;
        Debug.Log($"[SkillWorker] '{skill.GetType().Name}'��(��) ���� {slotToUse}�� �ڵ� �����߽��ϴ�.");
    }
}