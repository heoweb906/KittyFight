using UnityEngine;

public class PS_BloodHunger : Passive
{
    [Header("블러드 헝거 설정")]
    public int hitsPerHeal = 3;   // 적에게 3번 피해를 입힐 때마다
    public int healAmount = 2;    // 체력 2 회복

    private int hitCount = 0;
    private PlayerHealth ownerHealth;

    [Header("이펙트")]
    public GameObject objEffect_Use;

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
}