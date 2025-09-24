using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerAbility ability;

    [Header("Visual")]
    public Transform visualPivot;

    // ¹ßÆÇ¿ë
    //private Vector3 storedVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
    }

    public void AttachToPlatform(Transform platform)
    {
        if (platform != null)
        {
            transform.SetParent(platform);
            //storedVelocity = rb.velocity;
            //rb.isKinematic = true;
        }
        else
        {
            transform.SetParent(null);
            //rb.velocity = storedVelocity;
        }
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
            Vector3 moveDirection = new Vector3(input.x * ability.moveSpeed, rb.velocity.y, 0);
            rb.velocity = moveDirection;
        }

        if (input.x != 0)
        {
            Vector3 newForward = new Vector3(input.x, 0, 0);
            transform.forward = newForward;

            if (visualPivot != null)
            {
                float yaw = input.x > 0 ? 230f : 130f;
                visualPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
    }
}