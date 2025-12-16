using UnityEngine;

public class PS_SpeedRoulette : Passive
{
    public override int PassiveId => 101;

    [Header("룰렛 설정")]
    [Tooltip("몇 초마다 이동 속도를 다시 뽑을지")]
    public float intervalSeconds = 6f;

    [Tooltip("랜덤 이동 속도 최소값")]
    public float minMoveSpeed = 1.5f;

    [Tooltip("랜덤 이동 속도 최대값")]
    public float maxMoveSpeed = 12.5f;

    [Header("Effects")]
    [SerializeField] private GameObject upEffectPrefab;
    [SerializeField] private GameObject downEffectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;


    [Header("FX 판정 기준")]
    [Tooltip("speed <= threshold면 downEffect, 그 외 upEffect")]
    public float fxDownThreshold = 5f;


    private float timer = 0f;
    private float originalMoveSpeed;
    private bool hasSavedOriginal = false;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);

        if (ability != null && !hasSavedOriginal)
        {
            originalMoveSpeed = ability.moveSpeed;
            hasSavedOriginal = true;
        }

        timer = 0f;

        e.OnTick += OnTick;
        e.OnRoundStart += OnRoundStart;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnRoundStart -= OnRoundStart;

        if (IsAuthority && ability != null && hasSavedOriginal)
        {
            ability.moveSpeed = originalMoveSpeed;
        }

        base.Unsubscribe(e);
    }

    private void OnTick(float dt)
    {
        if (!IsAuthority) return;
        if (ability == null || intervalSeconds <= 0f) return;

        timer += dt;
        if (timer >= intervalSeconds)
        {
            timer -= intervalSeconds;
            RollNewSpeed();
        }
    }

    private void OnRoundStart(int roundIndex)
    {
        if (!IsAuthority) return;
        timer = 0f;      // 라운드 시작할 때 타이머 초기화
        RollNewSpeed();
    }

    private void RollNewSpeed()
    {
        if (ability == null) return;

        float newSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        ability.moveSpeed = newSpeed;

        bool isDown = newSpeed <= fxDownThreshold;
        PlayFx(isDown);

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);

        SendProc(
            PassiveProcType.FxOnly,
            pos: transform.position,
            dir: Vector3.up,
            i0: isDown ? 0 : 1,
            f0: newSpeed // 디버그용
        );
    }

    private void PlayFx(bool isDown)
    {
        var prefab = isDown ? downEffectPrefab : upEffectPrefab;
        if (prefab == null) return;

        Instantiate(prefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        bool isUp = msg.i0 == 1;
        bool isDown = !isUp;

        PlayFx(isDown);
    }
}