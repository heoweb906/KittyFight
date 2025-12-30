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

    Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
        anim = GetComponentInChildren<Animator>();
        jumpScript = GetComponent<PlayerJump>();

    }
    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (2.0f - 1) * Time.deltaTime;
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
        if (input.x != 0)
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
}