using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerDash))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerDash dash;
    private PlayerAbility ability;

    private Vector2 moveInput;
    private bool jumpInput;
    private bool meleeInput;
    private bool rangedInput;
    private bool dashInput;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        dash = GetComponent<PlayerDash>();
        ability = GetComponent<PlayerAbility>();
    }

    private void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        meleeInput = Input.GetMouseButtonDown(0);
        rangedInput = Input.GetMouseButtonDown(1);
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);

        if (jumpInput) jump.TryJump();
        if (dashInput) dash.TryDash();

        if (meleeInput) TryExecuteAimedSkill(SkillType.Melee);
        if (rangedInput) TryExecuteAimedSkill(SkillType.Ranged);
    }

    private void FixedUpdate()
    {
        movement.Move(moveInput);
        jump.HandleWallSlide();
    }

    private void TryExecuteAimedSkill(SkillType type)
    {
        var s = ability.GetSkill(type);
        if (s == null) return;

        float range = 3.5f;
        if (s is SK_MeleeAttack sm) range = sm.maxRange;
        else if (s is SK_RangedAttack sr) range = sr.maxRange;

        Vector3 origin, dir;
        AttackUtils.GetAimPointAndDirection(transform, range, out origin, out dir);
        ability.TryExecuteSkill(type, origin, dir);
    }
}