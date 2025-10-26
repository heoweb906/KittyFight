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
            // ���� ���� ������Ʈ�� �����̴� �����̶��, �ٴ� ó���� �߰�
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

            // ���� ������ ������Ʈ�� �����̴� �����̾��ٸ�, �ٴ� ó���� ����
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