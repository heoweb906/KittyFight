using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAbilityData", menuName = "ScriptableObjects/PlayerAbilityData", order = 1)]
public class PlayerAbilityData : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 2f;

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashCooldown = 1f;

    [Header("Attack")]
    public float attackCooldown = 0.5f;
    public float attackHitboxDuration = 0.2f;
}