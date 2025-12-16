using UnityEngine;

public class PS_FuryBlast : Passive
{
    public override int PassiveId => 113;

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

    // procType 구분용
    private const int PROC_CHARGE = 1;
    private const int PROC_BLAST = 2;

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

    private void OnTick(float dt)
    {
        if (!IsAuthority) return;
        if (ability == null) return;

        timer += dt;
        float chargeThreshold = Mathf.Max(0f, interval - chargeTime);

        if (!isCharging && timer >= chargeThreshold)
        {
            isCharging = true;

            SpawnChargeFx_Local();
            SendProc(
                PassiveProcType.FxOnly,
                pos: ability.transform.position,
                dir: Vector3.up,
                i0: PROC_CHARGE
            );
        }

        if (timer < interval) return;

        timer -= interval;
        isCharging = false;

        TriggerBlast_Local();
        SendProc(
            PassiveProcType.Spawn,
            pos: ability.transform.position,
            dir: Vector3.up,
            i0: PROC_BLAST
        );
    }

    private void SpawnChargeFx_Local()
    {
        if (!explosionFxPrefab) return;

        var pos = ability.transform.position;
        Object.Instantiate(
            explosionFxPrefab,
            pos,
            Quaternion.Euler(-90, 0, 0),
            transform
        );
    }

    private void TriggerBlast_Local()
    {
        var pos = ability.transform.position;

        if (blastHitboxPrefab)
        {
            var go = Object.Instantiate(blastHitboxPrefab, pos, Quaternion.identity);
            var hb = go.GetComponent<AB_HitboxBase>();
            if (hb != null) hb.Init(ability);
        }

        if (explosionFxPrefab)
        {
            Object.Instantiate(
                explosionFxPrefab,
                pos,
                Quaternion.Euler(-90, 0, 0)
            );
        }

        if (!selfDamageLocalOnly)
        {
            var myHp = ability.GetComponent<PlayerHealth>();
            if (myHp) myHp.TakeDamage(selfDamage);
        }

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        if (ability == null) return;

        var pos = new Vector3(msg.px, msg.py, msg.pz);

        if (msg.i0 == PROC_CHARGE)
        {
            if (!explosionFxPrefab) return;
            Object.Instantiate(
                explosionFxPrefab,
                pos,
                Quaternion.Euler(-90, 0, 0),
                transform
            );
            return;
        }

        if (msg.i0 == PROC_BLAST)
        {
            if (blastHitboxPrefab)
            {
                var go = Object.Instantiate(blastHitboxPrefab, pos, Quaternion.identity);
                var hb = go.GetComponent<AB_HitboxBase>();
                if (hb != null) hb.Init(ability);
            }

            if (explosionFxPrefab)
            {
                Object.Instantiate(
                    explosionFxPrefab,
                    pos,
                    Quaternion.Euler(-90, 0, 0)
                );
            }
        }
    }
}
