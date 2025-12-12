using UnityEngine;

public class PS_Kickstart : Passive
{
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
        // 진행 중인 모든 쿨타임을 즉시 0.6초 감소
        ability?.ReduceAllCooldowns(Mathf.Max(0f, reducePerJump));
        Instantiate(
            effectPrefab,
            transform.position,
            Quaternion.identity
        );

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }
}