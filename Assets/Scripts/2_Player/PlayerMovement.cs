using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerAbility ability;
    private Vector3 moveDirection;

    [Header("Visual")]
    public Transform visualPivot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
    }

    public void Move(Vector2 input)
    {
        moveDirection = new Vector3(input.x * ability.moveSpeed, rb.velocity.y, 0);
        rb.velocity = moveDirection;

        if (input.x != 0)
        {
            Vector3 newForward = new Vector3(input.x, 0, 0);
            transform.forward = newForward;

            if (visualPivot != null)
            {
                float yaw = input.x > 0 ? 50f : -50f;
                visualPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
    }
}