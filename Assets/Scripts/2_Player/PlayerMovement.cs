using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerAbility ability;
    private Animator anim;
    private PlayerJump jumpScript;

    [Header("Visual")]
    public Transform visualPivot;

    [Header("Parenting Platform")]
    public Transform playersRoot;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public float mulSlopeSpeed = 1.2f;

    [SerializeField] private Transform groundCheck;

    [Header("Skill Facing Lock")]
    private const float DEFAULT_SKILL_LOCK_SEC = 0.25f;
    private bool facingLocked;
    private float facingUnlockTime;
    private Vector3 lockedForward;
    private float lockedYaw;

    private Vector3 moveDirection;


    [Header("딱딱한 점프 설정")]
    public float fallMultiplier = 5.0f;      // 하강 시 중력 배수 (매우 높게)
    public float lowJumpMultiplier = 4.0f;   // 점프 버튼을 살짝 눌렀을 때 중력 배수
    public float jumpMultiplier = 2.0f;      // 상승 중 기본 중력 배수
    public float maxFallSpeed = 25f;         // 최대 낙하 속도 제한

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
        anim = GetComponentInChildren<Animator>();
        jumpScript = GetComponent<PlayerJump>();

    }


    void FixedUpdate()
    {
        ApplyCustomGravity();
    }

    private void ApplyCustomGravity()
    {
        // 1. 하강 중일 때 (가장 무겁게)
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // 2. 상승 중인데 점프 버튼을 뗐을 때 (가변 점프를 딱딱하게)
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        // 3. 일반 상승 중일 때 (기본 중력보다 더 무겁게)
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.deltaTime;
        }

        // 최고 낙하 속도 제한 (바닥 뚫기 방지)
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, -maxFallSpeed, rb.velocity.z);
        }
    }

    public void AttachToPlatform(Transform platform)
    {
        if (platform != null)
        {
            transform.SetParent(platform);
        }
        else
        {
            transform.SetParent(null);
        }
    }
    public void ForceDetachFromPlatform()
    {
        if (playersRoot != null)
            transform.SetParent(playersRoot, true);
        else
            transform.SetParent(null, true);
    }

    public void Move(Vector2 input)
    {
        bool grounded = (jumpScript != null && jumpScript.IsGrounded);
        bool isJump = (jumpScript != null && jumpScript.IsJump);

        moveDirection = new Vector3(input.x * ability.moveSpeed, rb.velocity.y, 0);
        bool onSlope = OnSlope() && grounded;

        if (rb.isKinematic)
        {
            float moveX = input.x * ability.moveSpeed * Time.deltaTime;
            transform.Translate(moveX, 0, 0, Space.World);
        }
        else if (OnSlope() && !isJump) {
            {
                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                moveDirection.y = 0f;
                Vector3 slopeDir = GetSlopeMoveDirection();
                rb.velocity = slopeDir * ability.moveSpeed * mulSlopeSpeed;
            }
        }
        else
        {
            rb.velocity = moveDirection;
        }

        rb.useGravity = !onSlope;

        anim.SetBool("isRun", input.x != 0f);

        bool locked = IsFacingLocked();
        if (locked) ApplyLockedFacing();

        if (!locked && input.x != 0)
        {
            Vector3 newForward = new Vector3(input.x, 0, 0);
            transform.forward = newForward;

            bool hanging = (jumpScript != null && jumpScript.IsTouchingWall);
            if (visualPivot != null && !hanging)
            {
                float yaw = input.x > 0 ? 50f : 310f;
                visualPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
    }

    private bool OnSlope()
    {
        Vector3 origin = groundCheck.position;
        Debug.DrawRay(origin, Vector3.down * 0.2f, Color.yellow);
        if (Physics.Raycast(origin, Vector3.down, out slopeHit, 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            Debug.Log($"Hit: {slopeHit.collider.name}, tag={slopeHit.collider.tag}, normal={slopeHit.normal}, angle={angle}");
            return angle < maxSlopeAngle && angle != 0;
        }
        Debug.Log(false);
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void LockFacing(Vector3 dir)
    {
        LockFacingInternal(dir, DEFAULT_SKILL_LOCK_SEC);
    }
    public void LockFacing(Vector3 dir, float duration)
    {
        LockFacingInternal(dir, duration);
    }

    private void LockFacingInternal(Vector3 dir, float duration)
    {
        if (dir.sqrMagnitude < 0.0001f) return;

        float x = dir.x;
        if (Mathf.Abs(x) < 0.0001f) return;

        lockedForward = new Vector3(x > 0f ? 1f : -1f, 0f, 0f);
        lockedYaw = lockedForward.x > 0f ? 50f : 310f;

        facingLocked = true;
        facingUnlockTime = Time.time + duration;

        ApplyLockedFacing();
    }

    private bool IsFacingLocked()
    {
        if (!facingLocked) return false;
        if (Time.time < facingUnlockTime) return true;

        facingLocked = false;
        return false;
    }

    private void ApplyLockedFacing()
    {
        transform.forward = lockedForward;

        if (visualPivot != null)
            visualPivot.localRotation = Quaternion.Euler(0f, lockedYaw, 0f);
    }
}