using UnityEngine;

public class PS_FuryBlast : Passive
{
    [Header("�ֱ�/����")]
    public float interval = 10f;        // 10�ʸ���
    public int selfDamage = 10;         // ������ 1 ������
    public bool selfDamageLocalOnly = true;

    [Header("������/����")]
    public GameObject blastHitboxPrefab;    // AB_FuryBlast ������
    public GameObject explosionFxPrefab;
    public float fxLife = 2f;

    private float timer;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        timer = 0f; // ���� �� interval �� ù �ߵ�
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

        // �ð� ȿ��
        if (explosionFxPrefab)
        {
            var fx = Object.Instantiate(explosionFxPrefab, pos, Quaternion.identity);
            Object.Destroy(fx, fxLife);
        }

        // ��Ʈ�ڽ�(Trigger) �� �� ���� �� �ֺ� ������ 2 ������
        if (blastHitboxPrefab)
        {
            var go = Object.Instantiate(blastHitboxPrefab, pos, Quaternion.identity);
            var hb = go.GetComponent<AB_HitboxBase>();
            if (hb != null) hb.Init(ability);
        }

        // �ڱ� �ڽ� 1 ������
        if (!selfDamageLocalOnly)
        {
            var myHp = ability.GetComponent<PlayerHealth>();
            if (myHp) myHp.TakeDamage(selfDamage);
        }
    }
}
