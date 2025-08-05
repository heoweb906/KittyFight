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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
    }

    public void TryJump()
    {
        if (isGrounded || isTouchingWall)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = ability.JumpForce;
            rb.velocity = velocity;
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
        isGrounded = value;
    }

    public void SetTouchingWall(bool value)
    {
        isTouchingWall = value;
    }
}