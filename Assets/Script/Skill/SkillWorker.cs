using System.Collections.Generic;
using UnityEngine;

public class SkillWorker : MonoBehaviour
{
    private Dictionary<SkillSlotType, Skill> mySkills = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) UseSkill(SkillSlotType.Q);
        if (Input.GetKeyDown(KeyCode.E)) UseSkill(SkillSlotType.E);
    }

    public void SetSkill(SkillSlotType slot, Skill skill)
    {
        mySkills[slot] = skill;
        Debug.Log($"[SkillWorker] Skill set in slot {slot}: {skill.GetType().Name}");
    }

    public void UseSkill(SkillSlotType slot)
    {
        if (mySkills.TryGetValue(slot, out Skill skill) && skill != null)
        {
            skill.Activate();
        }
        else
        {
            Debug.LogWarning($"[SkillWorker] No skill in slot {slot}");
        }
    }
}
