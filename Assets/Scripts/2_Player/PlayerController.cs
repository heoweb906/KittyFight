using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerAbility ability;

    private Vector2 moveInput;
    private bool jumpInput;
    private bool meleeInput;
    private bool rangedInput;
    private bool dashInput;
    private bool skill1Input;
    private bool skill2Input;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        ability = GetComponent<PlayerAbility>();
    }

    private void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        meleeInput = Input.GetMouseButtonDown(0);
        rangedInput = Input.GetMouseButtonDown(1);
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        skill1Input = Input.GetKeyDown(KeyCode.Q);
        skill2Input = Input.GetKeyDown(KeyCode.E);

        if (jumpInput) jump.TryJump();

        if (meleeInput) TryExecuteAimedSkill(SkillType.Melee);
        if (rangedInput) TryExecuteAimedSkill(SkillType.Ranged);
        if (dashInput) TryExecuteAimedSkill(SkillType.Dash);
        if (skill1Input) TryExecuteAimedSkill(SkillType.Skill1);
        if (skill2Input) TryExecuteAimedSkill(SkillType.Skill2);
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

        // Rabbit Hole 전용
        if (s is SK_RabbitHole rabbit)
        {
            Vector3 currentPos = transform.position;
            Vector3 clickPos = AttackUtils.GetWorldClickOnSameZ(transform);

            Vector3 targetPos = clickPos;
            targetPos.z = currentPos.z;

            // 여기서 막혀 있으면 아예 실행 안 함 => 쿨타임 X
            if (!rabbit.CanTeleportPosition(currentPos, targetPos))
                return;

            Vector3 dir1 = targetPos - currentPos;
            ability.TryExecuteSkill(type, currentPos, dir1);
            return;
        }

        float range = s.GetAimRange();
        Vector3 origin, dir;
        AttackUtils.GetAimPointAndDirection(transform, range, out origin, out dir);

        ability.TryExecuteSkill(type, origin, dir);
    }
}