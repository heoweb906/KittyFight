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

    public bool bCantInput { get; set; }

    private void Awake()
    {
        playerAbility = GetComponent<PlayerAbility>();
    }
    private void Update()
    {
        if (bCantInput) return;

        if (Input.GetKeyDown(KeyCode.Q)) UseSkill(SkillSlotType.Q);
        if (Input.GetKeyDown(KeyCode.E)) UseSkill(SkillSlotType.E);
    }



    public void EquipSkillByCard(GameObject skillObj)
    {
        if (skillObj == null)
        {
            Debug.LogWarning("[SkillWorker] Tried to equip a null skill object.");
            return;
        }

        Skill skillInstance = skillObj.GetComponent<Skill>();
        if (skillInstance == null)
        {
            Debug.LogError("[SkillWorker] Provided object has no Skill component.");
            Destroy(skillObj);
            return;
        }

        //skillInstance.SetNewBasicValue(playerAbility, this);

        SkillSlotType slotToUse =
            !mySkills.ContainsKey(SkillSlotType.Q) || mySkills[SkillSlotType.Q] == null
                ? SkillSlotType.Q
                : SkillSlotType.E;

        mySkills[slotToUse] = skillInstance;

        Debug.Log($"[SkillWorker] Skill '{skillInstance.name}' has been equipped to slot {slotToUse}.");
    }


    public void UseSkill(SkillSlotType slot)
    {
        //if (mySkills.TryGetValue(slot, out Skill skill) && skill != null) skill.Activate();
        //else Debug.Log($"[SkillWorker] No skill assigned in slot {slot}.");
        //return;
    }
}
