using UnityEngine;

public class SK_MastersTouch : Skill
{
    private void Awake()
    {
        coolTime = 8f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

        var ability = playerAbility;

        SkillType[] slots =
        {
            SkillType.Melee,
            SkillType.Ranged,
            SkillType.Dash,
            SkillType.Skill1,
            SkillType.Skill2
        };

        foreach (var slot in slots)
        {
            if (slot == assignedSlot) continue;
            ability.CancelCooldown(slot);
        }
    }
}