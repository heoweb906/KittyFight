using UnityEngine;

public class PS_Frenzy : Passive
{
    public override int PassiveId => 132;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        if (ability == null) return;

        ability.OnSkillEquipped += OnSkillEquipped;
        ForceConvertExistingSlots();

        e.OnMeleeHitboxSpawned += OnMeleeHitboxSpawned;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        if (ability != null)
        {
            ability.OnSkillEquipped -= OnSkillEquipped;
        }

        e.OnMeleeHitboxSpawned -= OnMeleeHitboxSpawned;
    }

    private void ForceConvertExistingSlots()
    {
        if (ability == null) return;

        ConvertSlotToMelee(SkillType.Ranged);
        ConvertSlotToMelee(SkillType.Skill1);
        ConvertSlotToMelee(SkillType.Skill2);
    }

    private void ConvertSlotToMelee(SkillType slot)
    {
        if (ability == null) return;

        var s = ability.GetSkill(slot);
        if (s == null) return;

        // SetSkill을 한 번 호출해서 OnSkillEquipped 이벤트를 발생시킴
        // 아래 OnSkillEquipped 핸들러에서 실제로 Melee로 갈아끼운다.
        ability.SetSkill(slot, s);
    }

    private void OnSkillEquipped(SkillType type, Skill skill)
    {
        if (ability == null) return;

        if (type == SkillType.Melee)
        {
            ForceConvertExistingSlots();
            return;
        }

        if (type == SkillType.Dash) return;

        var melee = ability.GetSkill(SkillType.Melee);
        if (melee == null) return;

        if (skill == melee) return;

        ability.SetSkill(type, melee);
    }

    private void OnMeleeHitboxSpawned(AB_MeleeHitbox hb)
    {
        if (hb == null) return;
        if (effectPrefab == null) return;

        Instantiate(
            effectPrefab,
            hb.transform.position,
            hb.transform.rotation * Quaternion.Euler(-120f, -90f, 90f)
        );
    }
}