using UnityEngine;

public class WallCheck : MonoBehaviour
{
    private PlayerJump jumpScript;

    private void Start()
    {
        jumpScript = GetComponentInParent<PlayerJump>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
            jumpScript.SetTouchingWall(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
            jumpScript.SetTouchingWall(false);
    }
}