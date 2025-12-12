using UnityEngine;

public class PS_FuryBlast : Passive
{
    [Header("주기/자해")]
    public float interval = 8f;        // 10초마다
    public int selfDamage = 10;         // 본인은 1 데미지
    public bool selfDamageLocalOnly = false;

    [Header("프리팹/연출")]
    public GameObject blastHitboxPrefab;    // AB_FuryBlast 프리팹
    public GameObject explosionFxPrefab;

    [Header("차징 설정")]
    public float chargeTime = 2f;

    private float timer;
    private bool isCharging;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        timer = 0f; // 장착 후 interval 뒤 첫 발동
        isCharging = false;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
    }

    void OnTick(float dt)
    {
        if (!ability) return;

        timer += dt;

        float chargeThreshold = Mathf.Max(0f, interval - chargeTime);

        if (!isCharging && timer >= chargeThreshold)
        {
            SpawnChargeFx();
            isCharging = true;
        }

        if (timer < interval) return;
        timer -= interval;
        isCharging = false;

        TriggerBlast();
    }
    void SpawnChargeFx()
    {
        if (!explosionFxPrefab) return;
        var pos = ability.transform.position;
        var fx = Object.Instantiate(
            explosionFxPrefab,
            pos,
            Quaternion.Euler(-90, 0, 0),
            transform
        );
    }

    void TriggerBlast()
    {
        var pos = ability.transform.position;

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

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }
}
