using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerJump jumpScript;

    private void Start()
    {
        jumpScript = GetComponentInParent<PlayerJump>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
            jumpScript.SetGrounded(true);
        if (other.CompareTag("Wall"))
            jumpScript.SetGrounded(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
            jumpScript.SetGrounded(false);
        if (other.CompareTag("Wall"))
            jumpScript.SetGrounded(false);
    }
}