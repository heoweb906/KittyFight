using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public PlayerAbilityData abilityData;

    public int playerNumber => abilityData.playerNumber;
    public float MoveSpeed => abilityData.moveSpeed;
    public float JumpForce => abilityData.jumpForce;
    public float DashDistance => abilityData.dashDistance;
    public float DashCooldown => abilityData.dashCooldown;
    public float AttackCooldown => abilityData.attackCooldown;
    public float AttackHitboxDuration => abilityData.attackHitboxDuration;
}