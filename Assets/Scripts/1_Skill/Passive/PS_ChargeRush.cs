using UnityEngine;

public class PS_ChargeRush : Passive
{
    [Header("Charge Settings")]
    [Tooltip("최대 충전 시간(초)")]
    public float maxChargeSeconds = 10f;   // 0~10
    [Tooltip("거리/속도 최소/최대 배수")]
    public float minMul = 1f;              // 1x
    public float maxMul = 3f;              // 3x

    public float idleSinceDash = 0f;
    float nextDashCooldown = 0f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private GameObject useEffectPrefab;

    [Header("Effect Scale")]
    [Tooltip("충전 0일 때 스케일 배수")]
    public float minScaleMul = 1f;
    [Tooltip("최대 충전일 때 스케일 배수")]
    public float maxScaleMul = 2f;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    // 인스턴스 된 이펙트 트랜스폼 / 기본 스케일
    private Transform effectInstance;
    private Vector3 effectBaseScale = Vector3.one;

    protected override void Subscribe(AbilityEvents e)
    {
        if (effectPrefab != null)
        {
            var fx = Instantiate(
                effectPrefab,
                transform.position,
                Quaternion.Euler(-90, 0, 0),
                transform
            );

            effectInstance = fx.transform;
            effectBaseScale = effectInstance.localScale;
        }

        // 1) 매 프레임 충전
        e.OnTick += OnTick;

        // 2) 대쉬 실행 직전 보정(거리/속도)
        e.OnDashWillExecute += OnDashWillExecute;

        // 3) 대쉬가 실행되어 쿨다운이 걸리면 리셋
        e.OnCooldownFinalized += OnCooldownFinalized;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnDashWillExecute -= OnDashWillExecute;
        e.OnCooldownFinalized -= OnCooldownFinalized;
    }

    void OnTick(float dt)
    {
        idleSinceDash = Mathf.Min(maxChargeSeconds, idleSinceDash + dt);
        if (effectInstance != null)
        {
            float charged = Mathf.Max(0f, idleSinceDash);
            float t = (maxChargeSeconds > 0f)
                ? Mathf.Clamp01(charged / maxChargeSeconds)
                : 0f;

            float scaleMul = Mathf.Lerp(minScaleMul, maxScaleMul, t);

            effectInstance.localScale = effectBaseScale * scaleMul;
        }
    }

    void OnCooldownFinalized(SkillType slot, float seconds)
    {
        if (slot != SkillType.Dash) return;

        nextDashCooldown = Mathf.Max(0f, seconds);
    }

    void OnDashWillExecute(ref DashParams p)
    {
        float charged = Mathf.Max(0f, idleSinceDash);
        float t = (maxChargeSeconds > 0f) ? Mathf.Clamp01(charged / maxChargeSeconds) : 0f;
        float mul = Mathf.Lerp(minMul, maxMul, t);

        Debug.Log(idleSinceDash);

        p.distance *= mul;
        p.speed *= mul;

        idleSinceDash = -nextDashCooldown;
        nextDashCooldown = 0f;

        Vector3 dir = p.direction;
        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angleDeg < 0f) angleDeg += 360f;
        Quaternion rot = Quaternion.Euler(angleDeg, -90f, 90f);

        Instantiate(
            useEffectPrefab,
            transform.position,
            rot
        );

        if (effectInstance != null)
        {
            effectInstance.localScale = effectBaseScale * minScaleMul;
        }



        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);

    }
}