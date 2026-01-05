using UnityEngine;

public class PS_Kickstart : Passive
{
    public override int PassiveId => 109;

    [Header("Kickstart Settings")]
    public float penaltyPerUse = 5.0f;   // 스킬 사용 시 +5초
    public float reducePerJump = 0.6f;   // 점프할 때마다 -0.6초(진행 중 쿨타임에 즉시 적용)

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnModifyCooldown += OnModifyCooldown; // 스킬 쿨타임 시작 직전 +5s
        e.OnJump += OnJump;                     // 점프/벽점프 시 진행 중 쿨타임 -0.6s
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;
        e.OnJump -= OnJump;
    }

    void OnModifyCooldown(SkillType slot, ref float seconds)
    {
        // 모든 스킬에 +5초 (필요 시 특정 슬롯만 필터링 가능)
        seconds += Mathf.Max(0f, penaltyPerUse);
    }

    void OnJump()
    {
        if (!IsAuthority) return;
        if (ability == null) return;

        float reduce = Mathf.Max(0f, reducePerJump);

        ability.ReduceAllCooldowns(reduce);

        PlayFx(transform.position);
        SendProc(
            PassiveProcType.Value,
            pos: transform.position,
            dir: Vector3.up,
            i0: 1,
            f0: reduce
        );

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    void PlayFx(Vector3 pos)
    {
        if (!effectPrefab) return;
        Instantiate(effectPrefab, pos, Quaternion.identity);

        ability.PlaySFX(audioClip);
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        if (ability == null) return;

        if (msg.i0 == 1)
        {
            float reduce = Mathf.Max(0f, msg.f0);
            ability.ReduceAllCooldowns(reduce);

            var pos = new Vector3(msg.px, msg.py, msg.pz);
            PlayFx(pos);
        }
    }
}