using UnityEngine;

public class PS_FeralBloodlust : Passive
{
    [Header("피해량 증폭 설정")]
    [Tooltip("적에게 주는 피해에 더해질 값")]
    public int extraDealDamage = 20;

    [Tooltip("내가 받는 피해에 더해질 값")]
    public int extraTakenDamage = 20;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnBeforeDealDamage += OnBeforeDealDamage;
        e.OnBeforeTakeDamage += OnBeforeTakeDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnBeforeDealDamage -= OnBeforeDealDamage;
        e.OnBeforeTakeDamage -= OnBeforeTakeDamage;
    }

    private void OnBeforeDealDamage(ref int dmg, GameObject victim)
    {
        if (dmg <= 0) return;
        if (extraDealDamage == 0) return;
        dmg += extraDealDamage;
    }

    private void OnBeforeTakeDamage(ref int dmg, GameObject attacker)
    {
        if (dmg <= 0) return;
        if (extraTakenDamage == 0) return;
        dmg += extraTakenDamage;
    }
}