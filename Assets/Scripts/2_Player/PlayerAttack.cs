using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerAbility))]
public class PlayerAttack : MonoBehaviour
{
    public GameObject hitboxPrefab;

    public float minSpawnDistance = 0.5f;   // 너무 가까운 경우 방지
    public float maxAttackRange = 3.5f;   // 최대 사거리

    public int myPlayerNumber;
    public SkillCooldownUI cooldownUI;

    private PlayerAbility ability;
    private bool canAttack = true;


    private void Awake()
    {
        ability = GetComponent<PlayerAbility>();
    }

    public void TryAttack()
    {
        if (canAttack)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        canAttack = false;

        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mp);
        Vector3 dir = worldClick - transform.position;

        if (dir.sqrMagnitude < 0.001f)
        {
            dir = transform.forward;
        }

        dir.Normalize();
        Vector3 spawnPos = transform.position + dir * maxAttackRange;
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, rot);

        P2PMessageSender.SendMessage(
            ObjectSpawnMessageBuilder.Build(spawnPos, rot, myPlayerNumber));

        InGameUIController.Instance?.StartSkillCooldown(myPlayerNumber);

        // 6) 지속시간 & 쿨타임
        yield return new WaitForSeconds(ability.AttackHitboxDuration);
        Destroy(hitbox);

        yield return new WaitForSeconds(ability.AttackCooldown);
        canAttack = true;
    }
}