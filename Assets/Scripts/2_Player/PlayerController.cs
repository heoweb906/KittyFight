using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerDash))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerAttack attack;
    private PlayerDash dash;

    private Vector2 moveInput;
    private bool jumpInput;
    private bool attackInput;
    private bool dashInput;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        attack = GetComponent<PlayerAttack>();
        dash = GetComponent<PlayerDash>();
    }

    private void Update()
    {
        // �Է� ����
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        attackInput = Input.GetMouseButtonDown(0);
        bool rangedInput = Input.GetMouseButtonDown(1);
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);

        // ���� �Է� ó��
        if (jumpInput)
            jump.TryJump();

        // ���� �Է� ó��
        if (attackInput)
            attack.TryMeleeAttack();
        if (rangedInput)
            attack.TryRangedAttack();
        if (dashInput)
            dash.TryDash();
    }

    private void FixedUpdate()
    {
        // �̵� ����
        movement.Move(moveInput);
        jump.HandleWallSlide();
    }
}