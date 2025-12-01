using UnityEngine;

public class PS_PiggyBank : Passive
{
    [Header("회복량 설정")]
    [Tooltip("스킬 사용 시 회복할 HP 양")]
    public int healAmount = 10;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnSkillExecuted += OnSkillExecuted;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnSkillExecuted -= OnSkillExecuted;
    }

    private void OnSkillExecuted(SkillType type)
    {
        if (ability == null) return;
        if (type == SkillType.Dash) return;

        var hp = ability.Health;
        if (hp == null) return;

        hp.Heal(healAmount);
    }
}