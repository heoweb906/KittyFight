using UnityEngine;

public class PS_BloodHunger : Passive
{
    [Header("블러드 헝거 설정")]
    public int hitsPerHeal = 3;   // 적에게 3번 피해를 입힐 때마다
    public int healAmount = 2;    // 체력 2 회복

    private int hitCount = 0;
    private PlayerHealth ownerHealth;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);

        if (ability != null)
            ownerHealth = ability.GetComponent<PlayerHealth>();

        e.OnDealtDamage += OnDealtDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnDealtDamage -= OnDealtDamage;
        base.Unsubscribe(e);
    }

    private void OnDealtDamage()
    {
        if (ownerHealth == null) return;

        hitCount++;

        if (hitCount >= hitsPerHeal)
        {
            hitCount -= hitsPerHeal;

            ownerHealth.Heal(healAmount);
        }
    }
}