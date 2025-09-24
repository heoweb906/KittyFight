using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]

public class TestPlayerComtroller : MonoBehaviour
{
    public MatchManager matchManager;

    public bool bCanControl;
    public TMP_Text text_nickname;

    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerAbility ability;

    private Vector2 moveInput;
    private bool jumpInput;
    private bool dashInput;
    private bool meleeInput;
    private bool rangedInput;


    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        ability = GetComponent<PlayerAbility>();

        text_nickname.text = matchManager.MyNickname;
    }

    private void Update()
    {
        if (!bCanControl) return;

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        meleeInput = Input.GetMouseButtonDown(0);
        rangedInput = Input.GetMouseButtonDown(1);

        if (jumpInput) jump.TryJump();
        if (dashInput) TryExecuteAimedSkill(SkillType.Dash);
        if (meleeInput) TryExecuteAimedSkill(SkillType.Melee);
        if (rangedInput) TryExecuteAimedSkill(SkillType.Ranged);
    }

    private void FixedUpdate()
    {
        if (!bCanControl) return;

        movement.Move(moveInput);
        jump.HandleWallSlide();
    }

    private void TryExecuteAimedSkill(SkillType type)
    {
        var s = ability.GetSkill(type);
        if (s == null) return;

        float range = s.GetAimRange();
        Vector3 origin, dir;
        AttackUtils.GetAimPointAndDirection(transform, range, out origin, out dir);

        ability.TryExecuteSkill(type, origin, dir);
    }
}
