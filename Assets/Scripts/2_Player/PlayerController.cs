using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerAttack attack;

    private Vector2 moveInput;
    private bool jumpInput;
    private bool attackInput;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        attack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        // �Է� ����
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        attackInput = Input.GetMouseButtonDown(0);

        // ���� �Է� ó��
        if (jumpInput)
        {
            jump.TryJump();
        }

        // ���� �Է� ó��
        if (attackInput)
        {
            attack.TryAttack();
        }
    }

    private void FixedUpdate()
    {
        // �̵� ����
        movement.Move(moveInput);
    }
}