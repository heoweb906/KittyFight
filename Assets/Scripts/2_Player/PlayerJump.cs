using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerJump : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerAbility ability;

    public LayerMask groundLayer;
    public LayerMask wallLayer;

    public Transform groundCheck;
    public float checkDistance = 0.6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
    }

    public void TryJump()
    {
        if (IsGrounded() || IsTouchingWall())
        //if(true)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = ability.JumpForce;
            rb.velocity = velocity;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(groundCheck.position, Vector3.down);
        bool hit = Physics.Raycast(ray, checkDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector3.down * checkDistance, hit ? Color.green : Color.red);
        return hit;
    }

    private bool IsTouchingWall()
    {
        Ray ray = new Ray(groundCheck.position, transform.forward);
        bool hit = Physics.Raycast(ray, checkDistance, wallLayer);
        Debug.DrawRay(groundCheck.position, transform.forward * checkDistance, hit ? Color.green : Color.red);
        return hit;
    }
}