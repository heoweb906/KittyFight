using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillSlotType
{
    Q,
    E
}

public class SkillManager : MonoBehaviour
{
    public SkillWorker targetWorker_1;
    public SkillWorker targetWorker_2; 

    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;
    public PlayerSkillAbilty playerSkillAbilty_1;
    public PlayerSkillAbilty playerSkillAbilty_2;

  

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            EquipSkillByName(1, SkillSlotType.Q, "FireBall");

        if (Input.GetKeyDown(KeyCode.T))
            EquipSkillByName(1, SkillSlotType.E, "IceArrow");
    }


    public void EquipSkillByName(int iPlayerID, SkillSlotType slot, string skillName)
    {
        Skill newSkill = null;

        switch (skillName)
        {
            case "FireBall":
                newSkill = new FireBall(playerAbility_1, playerSkillAbilty_1);
                break;
            case "IceArrow":
                newSkill = new IceArrow(playerAbility_2, playerSkillAbilty_2);
                break;
            default:
                Debug.LogWarning($"[SkillManager] Unknown skill name: {skillName}");
                return;
        }


        if(iPlayerID == 1 && targetWorker_1) targetWorker_1.SetSkill(slot, newSkill);
        else if (iPlayerID == 2 && targetWorker_2) targetWorker_2.SetSkill(slot, newSkill);

        Debug.Log($"[SkillManager] Skill '{skillName}' equipped to slot {slot}");
    }
}
