using UnityEngine;

public class PS_FeralBloodlust : Passive
{
    [Header("피해량 증폭 설정")]
    [Tooltip("적에게 주는 피해에 더해질 값")]
    public int extraDealDamage = 20;

    [Tooltip("내가 받는 피해에 더해질 값")]
    public int extraTakenDamage = 20;

    [Header("이펙트")]
    public GameObject objEffect_Use;

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

        // 피해 입혔을 때 상대방에게서도 이펙트 생성해야 함
        Instantiate(
            objEffect_Use,
            victim.transform.position,
            Quaternion.Euler(-90f, 0f, 0f)
        );
    }

    private void OnBeforeTakeDamage(ref int dmg, GameObject attacker)
    {
        if (dmg <= 0) return;
        if (extraTakenDamage == 0) return;
        dmg += extraTakenDamage;

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