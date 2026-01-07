using UnityEngine;

public class PS_DataDump : Passive
{
    public override int PassiveId => 127;

    [Header("쿨타임 초기화 주기")]
    [Tooltip("몇 초마다 한 번씩 모든 스킬 쿨타임을 초기화할지")]
    public float interval = 15f;
    private float timer = 0f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    private const int PROC_RESET = 1;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        e.OnRoundStart += OnRoundStart;
        e.OnRoundEnd += OnRoundEnd;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnRoundStart -= OnRoundStart;
        e.OnRoundEnd -= OnRoundEnd;
    }

    private void OnRoundStart(int roundIndex)
    {
        if (!IsAuthority) return;
        timer = 0f;
    }
    private void OnRoundEnd(int roundIndex)
    {
        if (!IsAuthority) return;
        timer = -999f;
    }

    private void OnTick(float dt)
    {
        if (!IsAuthority) return; 
        if (ability == null) return;
        if (interval <= 0f) return;

        timer += dt;
        if (timer >= interval)
        {
            timer -= interval;

            ability.ResetAllCooldowns();

            PlayFx(transform.position);
            SendProc(
                PassiveProcType.Value,
                pos: transform.position,
                dir: Vector3.up,
                i0: PROC_RESET,
                f0: 0f
            );

            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
        }
    }

    private void PlayFx(Vector3 pos)
    {
        if (!effectPrefab) return;

        Instantiate(
            effectPrefab,
            pos,
            Quaternion.Euler(-90f, 0f, 0f)
        );

        ability.PlaySFX(audioClip);
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        if (ability == null) return;
        if (msg.i0 != PROC_RESET) return;

        ability.ResetAllCooldowns();
        var pos = new Vector3(msg.px, msg.py, msg.pz);
        PlayFx(pos);
    }
}