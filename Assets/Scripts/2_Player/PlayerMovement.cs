using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerAbility ability;
    private Animator anim;

    [Header("Visual")]
    public Transform visualPivot;

    [Header("Parenting Platform")]
    public Transform playersRoot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ability = GetComponent<PlayerAbility>();
        anim = GetComponentInChildren<Animator>();
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
            Vector3 moveDirection = new Vector3(input.x * ability.moveSpeed, rb.velocity.y, 0);
            rb.velocity = moveDirection;
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
}