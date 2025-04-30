using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public SkillActionManager skillActionManager;
    public PlayerAbility playerAbility;









    private void Awake()
    {
        skillActionManager = GetComponent<SkillActionManager>();
        playerAbility = GetComponent<PlayerAbility>();
    }

    private void Update()
    {
        HandleInput();
        WallSlide();
        WallJump();

        if (!playerAbility.isWallJumping)
            Flip();

        Flip();
        DrawMouseRay();
    }

    private void FixedUpdate()
    {
        if (playerAbility.isDashing) return;

        float moveDir = playerAbility.horizontal;

        if (IsWalled())
        {
            if (playerAbility.isFacingRight && moveDir > 0f) moveDir = 0f;
            if (!playerAbility.isFacingRight && moveDir < 0f) moveDir = 0f;
        }

        Vector3 velocity = playerAbility.rigid.velocity;
        velocity.x = moveDir * playerAbility.speed;

        if (IsGrounded() && velocity.y < 0f)
            velocity.y = 0f;

        playerAbility.rigid.velocity = velocity;
    }

    private void HandleInput()
    {
        playerAbility.horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            playerAbility.rigid.velocity = new Vector3(
                playerAbility.rigid.velocity.x,
                playerAbility.jumpingPower,
                playerAbility.rigid.velocity.z
            );
        }

        if (Input.GetButtonUp("Jump") && playerAbility.rigid.velocity.y > 0f)
        {
            playerAbility.rigid.velocity = new Vector3(
                playerAbility.rigid.velocity.x,
                playerAbility.rigid.velocity.y * 0.5f,
                playerAbility.rigid.velocity.z
            );
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && playerAbility.canDash)
            StartCoroutine(Dash());

        if (Input.GetKeyDown(KeyCode.Q))
            skillActionManager.ExecuteAction(PlayerActionType.SkillQ);

        if (Input.GetKeyDown(KeyCode.E))
            skillActionManager.ExecuteAction(PlayerActionType.SkillE);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(playerAbility.groundCheck.position, 0.05f, playerAbility.groundLayer);
    }

    private bool IsWalled()
    {
        return Physics.CheckSphere(playerAbility.wallCheck.position, 0.1f, playerAbility.wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && playerAbility.horizontal != 0f)
        {
            playerAbility.isWallSliding = true;

            Vector3 velocity = playerAbility.rigid.velocity;
            velocity.y = Mathf.Clamp(velocity.y, -playerAbility.wallSlidingSpeed, float.MaxValue);
            playerAbility.rigid.velocity = velocity;
        }
        else
        {
            playerAbility.isWallSliding = false;
        }
    }

    private void WallJump()
    {
        bool isOnWall = playerAbility.isWallSliding && Mathf.Abs(playerAbility.horizontal) > 0.1f;

        if (isOnWall)
        {
            playerAbility.wallJumpingCounter = playerAbility.wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            playerAbility.wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && playerAbility.wallJumpingCounter > 0f)
        {
            playerAbility.isWallJumping = true;

            Vector3 jumpDir;

            if (IsWalled() && IsFacingWall())
            {
                jumpDir = new Vector3(0f, playerAbility.wallJumpingPower.y, 0f);
            }
            else
            {
                jumpDir = new Vector3(
                    playerAbility.rigid.velocity.x,
                    playerAbility.jumpingPower,
                    playerAbility.rigid.velocity.z
                );
            }

            playerAbility.rigid.velocity = jumpDir;
            playerAbility.wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), playerAbility.wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        playerAbility.isWallJumping = false;
    }

    private bool IsFacingWall()
    {
        return (playerAbility.isFacingRight && Physics.CheckSphere(playerAbility.wallCheck.position, 0.1f, playerAbility.wallLayer)) ||
               (!playerAbility.isFacingRight && Physics.CheckSphere(playerAbility.wallCheck.position, 0.1f, playerAbility.wallLayer));
    }

    private void Flip()
    {
        if ((playerAbility.isFacingRight && playerAbility.horizontal < 0f) || (!playerAbility.isFacingRight && playerAbility.horizontal > 0f))
        {
            playerAbility.isFacingRight = !playerAbility.isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        playerAbility.canDash = false;
        playerAbility.isDashing = true;
        playerAbility.rigid.useGravity = false;

        Vector3 dashDirection = GetMouseDirection();
        playerAbility.rigid.velocity = dashDirection * playerAbility.dashingPower;

        Debug.DrawRay(transform.position, dashDirection * 3f, Color.red, 1f);

        yield return new WaitForSeconds(playerAbility.dashingTime);

        playerAbility.rigid.useGravity = true;
        playerAbility.isDashing = false;
        playerAbility.rigid.velocity = Vector3.zero;

        yield return new WaitForSeconds(playerAbility.dashingCooldown);
        playerAbility.canDash = true;
    }

    private Vector3 GetMouseDirection()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = (mouseWorldPos - transform.position);
        direction.z = 0f;

        return direction.normalized;
    }

    private void DrawMouseRay()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = mouseWorldPos - transform.position;
        direction.z = 0f;

        Debug.DrawRay(transform.position, direction, Color.green);
    }
}
