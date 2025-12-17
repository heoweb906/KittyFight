using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerJump : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerAbility ability;
    private PlayerMovement movementScript;
    private Animator anim;

    private bool isJump = false;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    public bool IsGrounded => isGrounded;
    public bool IsJump => isJump;

    public float wallSlideSpeed = 0.5f;

    [Header("Effects")]
    [SerializeField] public GameObject jumpEffectPrefab;
    [SerializeField] public GameObject landEffectPrefab;  // 착지 이펙트
    [SerializeField] public Transform effectAnchor;       // 발밑 기준 위치

    [Header("Walk (Loop FX)")]
    [SerializeField] private ParticleSystem walkLoopFx;
    [SerializeField] private float walkSpeedThreshold = 0.1f;
    public bool IsWalking { get; private set; }
    public bool IsWalking2 = false;

    private Coroutine jumpResetRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
        movementScript = GetComponent<PlayerMovement>();
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        anim.SetBool("isGround", isGrounded);
        anim.SetBool("isHanging", isTouchingWall);
        anim.SetFloat("speedY", rb.velocity.y);

        if (isTouchingWall)
        {
            bool isHangRight = transform.forward.x < 0f;
            anim.SetBool("isHangRight", isHangRight);
        }

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
            isJump = true;
            movementScript.AttachToPlatform(null);

            Vector3 velocity = rb.velocity;
            velocity.y = ability.jumpForce;
            rb.velocity = velocity;

            // PS_KickStart
            ability.events?.EmitJump();

            UpdateManager.EnqueueEventOnce("Jump");


            if (jumpResetRoutine != null)
            {
                StopCoroutine(jumpResetRoutine);
            }
            jumpResetRoutine = StartCoroutine(ResetJumpFlagAfterDelay(0.1f));

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

    private IEnumerator ResetJumpFlagAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isJump = false;
        jumpResetRoutine = null;
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
                Instantiate(landEffectPrefab, effectAnchor.position, Quaternion.identity);

                //var cm = FindObjectOfType<CameraManager>();
                //cm?.ShakeCameraPunch(0.02f, 0.7f, Vector3.down);

            }
        }
    }

    public void SetTouchingWall(bool value)
    {
        isTouchingWall = value;
    }
}