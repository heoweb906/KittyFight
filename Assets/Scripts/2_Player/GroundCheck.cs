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
        // �߹ؿ� ���� Wall�� ������ ���
        if (other.CompareTag("Wall"))
        {
            jumpScript.SetGrounded(true);

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
            jumpScript.SetGrounded(false);

            // ���� ������ ������Ʈ�� �����̴� �����̾��ٸ�, �ٴ� ó���� ����
            if (other.GetComponent<MovingPlatform>() != null)
            {
                movementScript.AttachToPlatform(null);
            }
        }
    }
}
