using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerJump : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerAbility ability;

    private bool isGrounded = false;
    private bool isTouchingWall = false;

    public float wallSlideSpeed = 0.5f;

    [Header("Effects")]
    [SerializeField] public GameObject jumpEffectPrefab;
    [SerializeField] private GameObject landEffectPrefab;  // 착지 이펙트
    [SerializeField] private Transform effectAnchor;       // 발밑 기준 위치

    [Header("Walk (Loop FX)")]
    [SerializeField] private ParticleSystem walkLoopFx;
    [SerializeField] private float walkSpeedThreshold = 0.1f;
    public bool IsWalking { get; private set; }
    public bool IsWalking2 = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
    }

    private void FixedUpdate()
    {
        IsWalking = isGrounded && Mathf.Abs(rb.velocity.x) > walkSpeedThreshold;

        if (walkLoopFx)
        {
            if (IsWalking || IsWalking2)
            {
                if (!walkLoopFx.isPlaying) walkLoopFx.Play();
            }
            else
            {
                if (walkLoopFx.isPlaying) walkLoopFx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    public void SetWalking(bool flag)
    {
        IsWalking2 = flag;
    }

    public void TryJump()
    {
        if (isGrounded || isTouchingWall)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = ability.jumpForce;
            rb.velocity = velocity;

            // PS_KickStart
            ability.events?.EmitJump();

            UpdateManager.EnqueueEventOnce("Jump");

            // 이펙트 생성
            if (jumpEffectPrefab != null)
            {
                Instantiate(
                    jumpEffectPrefab,
                    transform.position,      // 플레이어 위치
                    Quaternion.Euler(-90f, 0f, 0f)
                );
            }
        }
    }

    public void HandleWallSlide()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < -wallSlideSpeed)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = -wallSlideSpeed;
            rb.velocity = velocity;
        }
    }

    public void SetGrounded(bool value)
    {
        bool wasGrounded = isGrounded;
        isGrounded = value;

        if (!wasGrounded && isGrounded)
        {
            if (landEffectPrefab != null)
            {
                Instantiate(
                    landEffectPrefab,
                    effectAnchor.position,
                    Quaternion.identity
                );
            }
        }
    }

    public void SetTouchingWall(bool value)
    {
        isTouchingWall = value;
    }
}