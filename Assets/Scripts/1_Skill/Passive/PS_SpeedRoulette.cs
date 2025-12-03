using UnityEngine;

public class PS_SpeedRoulette : Passive
{
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

        if (ability != null && hasSavedOriginal)
        {
            ability.moveSpeed = originalMoveSpeed;
        }

        base.Unsubscribe(e);
    }

    private void OnTick(float dt)
    {
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
        timer = 0f;      // 라운드 시작할 때 타이머 초기화
        RollNewSpeed();
    }

    private void RollNewSpeed()
    {
        if (ability == null) return;

        float newSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        if (newSpeed <= 5)
        {
            Instantiate(
                downEffectPrefab,
                transform.position,
                Quaternion.Euler(-90f, 0f, 0f)
            );
        }
        else
        {
            Instantiate(
                upEffectPrefab,
                transform.position,
                Quaternion.Euler(-90f, 0f, 0f)
            );
        }
        ability.moveSpeed = newSpeed;
    }
}