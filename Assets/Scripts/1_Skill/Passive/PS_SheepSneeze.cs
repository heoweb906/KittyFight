using UnityEngine;

/// <summary>
/// 5초마다 플레이어 위치 기준 좌측 상단 / 우측 상단 방향으로
/// 털뭉치(AB_SheepSneeze)를 자동으로 소환하는 패시브.
/// </summary>
public class PS_SheepSneeze : Passive
{
    public override int PassiveId => 122;

    [Header("발사 설정")]
    [Tooltip("털뭉치로 사용할 프리팹(AB_SheepSneeze가 붙어 있는 오브젝트)")]
    public GameObject objSheepSneeze;

    [Tooltip("발사 주기(초) - 5초마다 자동 발사")]
    public float interval = 5f;

    [Tooltip("플레이어 위치 기준으로 얼마나 떨어진 곳에 생성할지")]
    public float spawnOffsetDistance = 1.0f;

    [Tooltip("플레이어 중심에서 위로 얼마나 올려서 생성할지")]
    public float verticalOffset = 0.5f;

    private float _timer = 0f;

    [Header("Effects")]
    [SerializeField] private GameObject useEffectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    private const int PROC_FIRE = 1;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        e.OnRoundStart += OnRoundStart;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnRoundStart -= OnRoundStart;
    }

    private void OnRoundStart(int roundIndex)
    {
        if (!IsAuthority) return;
        _timer = 0f;
    }

    private void OnTick(float dt)
    {
        if (!IsAuthority) return;
        if (ability == null) return;
        if (objSheepSneeze == null) return;

        _timer += dt;
        if (_timer >= interval)
        {
            _timer -= interval;

            SpawnHairballs();

            SendProc(
                PassiveProcType.Spawn,
                pos: ability.transform.position,
                dir: Vector3.up,
                i0: PROC_FIRE,
                f0: 0f
            );
        }
    }

    private void SpawnHairballs()
    {
        if (useEffectPrefab != null)
        {
            Instantiate(
                useEffectPrefab,
                transform.position,
                Quaternion.Euler(-90f, 0f, 0f)
            );
        }

        Transform t = ability.transform;
        Vector3 origin = t.position + Vector3.up * verticalOffset;

        Vector3 dirLeft = new Vector3(-1f, 1f, 0f).normalized;
        Vector3 dirRight = new Vector3(1f, 1f, 0f).normalized;

        ability.PlaySFX(audioClip);
        


        SpawnOne(origin, dirLeft);
        SpawnOne(origin, dirRight);

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    private void SpawnOne(Vector3 origin, Vector3 dir)
    {
        Vector3 spawnPos = origin + dir * spawnOffsetDistance;

        var go = Object.Instantiate(objSheepSneeze, spawnPos, Quaternion.identity);

        var hb = go.GetComponent<AB_HitboxBase>();
        if (hb != null) hb.Init(ability);

        var floater = go.GetComponent<SheepSneezeFloat>();
        if (floater == null)
            floater = go.AddComponent<SheepSneezeFloat>();

        floater.Init(dir);

        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.RegisterRoundObject(go);
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        if (ability == null) return;
        if (objSheepSneeze == null) return;

        if (msg.i0 != PROC_FIRE) return;

        SpawnHairballs();
    }
}