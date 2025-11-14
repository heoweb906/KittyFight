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

    [Header("Slope Move")]
    [SerializeField] private float groundRayDistance = 0.6f;   // 발밑 Raycast 거리
    [SerializeField] private float groundRayOffsetY = 0.1f;

    [Header("Ground Move")]
    [SerializeField] private float groundStickForce = 5f;  // 경사에서 살짝 아래로 눌러주는 힘

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
        anim = GetComponentInChildren<Animator>();
        jumpScript = GetComponent<PlayerJump>();
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
        if (rb.isKinematic)
        {
            float moveX = input.x * ability.moveSpeed * Time.deltaTime;
            transform.Translate(moveX, 0, 0, Space.World);
        }
        else
        {
            HandleDynamicMove(input);
        }

        anim.SetBool("isRun", input.x != 0f);
        if (input.x != 0)
        {
            Vector3 newForward = new Vector3(input.x, 0, 0);
            transform.forward = newForward;

            if (visualPivot != null)
            {
                float yaw = input.x > 0 ? 50f : 310f;
                visualPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
    }
    private void HandleDynamicMove(Vector2 input)
    {
        bool grounded = (jumpScript != null && jumpScript.IsGrounded);
        bool inJumpLock = (jumpScript != null && jumpScript.IsInJumpLock);

        if (grounded && !inJumpLock)
        {
            rb.useGravity = false;

            float moveX = input.x * ability.moveSpeed;
            float moveY = 0f;

            if (Mathf.Abs(input.x) > 0.001f)
            {
                moveY = -groundStickForce;
            }

            rb.velocity = new Vector3(moveX, moveY, 0f);
        }
        else
        {
            // 공중, 점프 락 중
            ApplyAirMove(input);
        }
    }

    private void ApplyAirMove(Vector2 input)
    {
        rb.useGravity = true;

        Vector3 moveVelocity = new Vector3(input.x * ability.moveSpeed, rb.velocity.y, 0f);
        rb.velocity = moveVelocity;
    }
}