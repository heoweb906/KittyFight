using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerJump jumpScript;
    private PlayerMovement movementScript;

    private void Start()
    {
        jumpScript = GetComponentInParent<PlayerJump>();
        movementScript = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 발밑에 닿은 Wall은 땅으로 취급
        if (other.CompareTag("Wall"))
        {
            jumpScript.SetGrounded(true);

            // 만약 닿은 오브젝트가 움직이는 발판이라면, 붙는 처리를 추가
            if (other.GetComponent<MovingPlatform>() != null)
            {
                movementScript.AttachToPlatform(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            jumpScript.SetGrounded(false);

            // 만약 떨어진 오브젝트가 움직이는 발판이었다면, 붙는 처리를 해제
            if (other.GetComponent<MovingPlatform>() != null)
            {
                movementScript.AttachToPlatform(null);
            }
        }
    }
}
