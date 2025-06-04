using UnityEngine;

public class PlayerInputRouter : MonoBehaviour
{
    private bool isMine = false;

    public void SetOwnership(bool mine)
    {
        isMine = mine;
        SetInputActive(isMine);
    }

    private void SetInputActive(bool active)
    {
        GetComponent<PlayerController>().enabled = active;
        GetComponent<PlayerMovement>().enabled = active;
        GetComponent<PlayerJump>().enabled = active;
        GetComponent<PlayerAttack>().enabled = active;
    }
}