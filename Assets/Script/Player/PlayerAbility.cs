using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public bool canAttack = true;
    public float attackCooldown = 1f;

    [Header("점프")]
    public float horizontal;
    public float speed = 8f;
    public float jumpingPower = 16f;
    public bool isFacingRight = true;

    [Header("벽 점프")]
    public bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    public bool isWallJumping;
    public float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    public float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;
    public Vector3 wallJumpingPower = new Vector3(8f, 16f, 0f);


    [Header("대쉬")]
    public bool canDash = true;
    public bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;


    [SerializeField] public Rigidbody rigid;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public Transform wallCheck;
    [SerializeField] public LayerMask wallLayer;


    [Header("스킬")]
    public string sSkillName_1;
    public string sSkillName_2;





}
