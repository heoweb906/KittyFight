using UnityEngine;

public class WallCheck : MonoBehaviour
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
        if (other.CompareTag("Wall"))
        {
            jumpScript.SetTouchingWall(true);
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
            jumpScript.SetTouchingWall(false);

            // 만약 떨어진 오브젝트가 움직이는 발판이었다면, 붙는 처리를 해제
            if (other.GetComponent<MovingPlatform>() != null)
            {
                movementScript.AttachToPlatform(null);
            }
        }
    }

    public void ForceClearContacts()
    {
        jumpScript?.SetTouchingWall(false);
        movementScript?.AttachToPlatform(null);
    }
}