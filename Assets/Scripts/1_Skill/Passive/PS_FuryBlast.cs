using UnityEngine;

public class PS_FuryBlast : Passive
{
    [Header("주기/자해")]
    public float interval = 10f;        // 10초마다
    public int selfDamage = 10;         // 본인은 1 데미지
    public bool selfDamageLocalOnly = true;

    [Header("프리팹/연출")]
    public GameObject blastHitboxPrefab;    // AB_FuryBlast 프리팹
    public GameObject explosionFxPrefab;
    public float fxLife = 2f;

    private float timer;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        timer = 0f; // 장착 후 interval 뒤 첫 발동
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
    }

    void OnTick(float dt)
    {
        if (!ability) return;

        timer += dt;
        if (timer < interval) return;
        timer -= interval;

        TriggerBlast();
    }

    void TriggerBlast()
    {
        var pos = ability.transform.position;

        // 시각 효과
        if (explosionFxPrefab)
        {
            var fx = Object.Instantiate(explosionFxPrefab, pos, Quaternion.identity);
            Object.Destroy(fx, fxLife);
        }

        // 히트박스(Trigger) 한 번 스폰 → 주변 적에게 2 데미지
        if (blastHitboxPrefab)
        {
            var go = Object.Instantiate(blastHitboxPrefab, pos, Quaternion.identity);
            var hb = go.GetComponent<AB_HitboxBase>();
            if (hb != null) hb.Init(ability);
        }

        // 자기 자신 1 데미지
        if (!selfDamageLocalOnly)
        {
            var myHp = ability.GetComponent<PlayerHealth>();
            if (myHp) myHp.TakeDamage(selfDamage);
        }
    }
}
