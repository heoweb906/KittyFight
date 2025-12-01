using UnityEngine;

public class PS_WoollyGuard : Passive
{
    [Header("받는 피해 감소 설정")]
    [Tooltip("받는 피해 감소")]
    public int damageDivisor = 12;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnBeforeTakeDamage += OnBeforeTakeDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnBeforeTakeDamage -= OnBeforeTakeDamage;
    }

    private void OnBeforeTakeDamage(ref int dmg, GameObject attacker)
    {
        if (dmg <= 0) return;
        if (damageDivisor <= 0f) return;

        dmg -= damageDivisor;
    }
}