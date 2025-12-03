using UnityEngine;

public class PS_PiggyBank : Passive
{
    [Header("회복량 설정")]
    [Tooltip("스킬 사용 시 회복할 HP 양")]
    public int healAmount = 10;

    [Header("이펙트")]
    public GameObject objEffect_Use;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnSkillExecuted += OnSkillExecuted;
        Debug.Log("구독됨");
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

        if (objEffect_Use != null)
        {
            GameObject effect = Instantiate(objEffect_Use, transform);
            effect.transform.localPosition = Vector3.zero;

            effect.transform.rotation = Quaternion.Euler(-90, 0, 0);

            effect.transform.localScale = new Vector3(1f, 1f, 1f);
            effect.transform.SetParent(null);
        }


    }
}