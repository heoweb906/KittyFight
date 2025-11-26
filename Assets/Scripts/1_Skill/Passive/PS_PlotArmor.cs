using UnityEngine;

public class PS_PlotArmor : Passive
{
    [Header("ÁãÅÐ °©¿Ê ¼³Á¤")]
    [Range(0f, 1f)]
    [Tooltip("ÇÇ°Ý ¹«½Ã È®·ü (0~1)")]
    public float ignoreChance = 0.25f;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnBeforeDealDamage += OnBeforeDealDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnBeforeDealDamage -= OnBeforeDealDamage;
        base.Unsubscribe(e);
    }

    private void OnBeforeDealDamage(ref int dmg, GameObject victim)
    {
        if (dmg <= 0 || victim == null) return;
        if (ability == null) return;
        if (victim != ability.gameObject) return;
        if (Random.value < ignoreChance)
        {
            dmg = 0;
        }
    }
}