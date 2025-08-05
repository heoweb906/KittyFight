using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerAbility))]
public class PlayerAttack : MonoBehaviour
{
    public GameObject hitboxPrefab;
    public GameObject projectilePrefab;

    public float maxAttackRange = 3.5f;
    public float projectileSpeed = 10f;

    public float meleeCooldown = 3.0f;
    public float rangedCooldown = 3.0f;

    public int myPlayerNumber;

    private PlayerAbility ability;
    private bool canMeleeAttack = true;
    private bool canRangedAttack = true;

    private void Awake()
    {
        ability = GetComponent<PlayerAbility>();
    }

    public void TryMeleeAttack()
    {
        if (!canMeleeAttack) return;
        canMeleeAttack = false;

        Vector3 aimPos, dir;
        AttackUtils.GetAimPointAndDirection(transform, maxAttackRange, out aimPos, out dir);

        Vector3 spawnPos = transform.position + dir * maxAttackRange;
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, rot);
        P2PMessageSender.SendMessage(
            ObjectSpawnMessageBuilder.Build(spawnPos, rot, myPlayerNumber));

        InGameUIController.Instance?.StartSkillCooldown(myPlayerNumber, 1);

        StartCoroutine(MeleeRoutine(hitbox));
        StartCoroutine(CooldownRoutine(() => canMeleeAttack = true, meleeCooldown));
    }

    public void TryRangedAttack()
    {
        if (!canRangedAttack) return;
        canRangedAttack = false;

        Vector3 aimPos, dir;
        AttackUtils.GetAimPointAndDirection(transform, maxAttackRange, out aimPos, out dir);

        Vector3 spawnPos = transform.position + dir * maxAttackRange;
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rot);
        proj.GetComponent<Rigidbody>().velocity = dir * projectileSpeed;

        // 메시지에 계산된 위치 포함
        P2PMessageSender.SendMessage(
            ProjectileMessageBuilder.Build(spawnPos, dir, projectileSpeed, myPlayerNumber));

        InGameUIController.Instance?.StartSkillCooldown(myPlayerNumber, 2);
        StartCoroutine(CooldownRoutine(() => canRangedAttack = true, rangedCooldown));
    }

    private IEnumerator MeleeRoutine(GameObject hitbox)
    {
        yield return new WaitForSeconds(ability.AttackHitboxDuration);
        if (hitbox != null) Destroy(hitbox);
    }

    private IEnumerator CooldownRoutine(System.Action onCooldownEnd, float duration)
    {
        yield return new WaitForSeconds(duration);
        onCooldownEnd?.Invoke();
    }
}