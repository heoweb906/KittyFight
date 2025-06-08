using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerAbility))]
public class PlayerAttack : MonoBehaviour
{
    public GameObject hitboxPrefab;

    public float minSpawnDistance = 0.5f;   // 너무 가까운 경우 방지
    public float maxAttackRange = 3.5f;   // 최대 사거리

    public int myPlayerNumber;

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

        /* -------- 1. 마우스 → 월드 방향 계산 -------- */
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector3 dir = mouseWorld - transform.position;
        dir.y = 0f;                                // 수평 방향만 사용
        if (dir == Vector3.zero) dir = transform.forward; // 마우스가 정확히 위라면 임시로 전방 사용
        dir.Normalize();

        /* -------- 2. 사거리 클램프 -------- */
        float spawnDistance = Mathf.Clamp(Vector3.Distance(transform.position, mouseWorld),
                                          minSpawnDistance,
                                          maxAttackRange);

        /* -------- 3. 히트박스 생성 (플레이어는 회전 X) -------- */
        Vector3 spawnPos = transform.position + dir * spawnDistance;
        Quaternion rot = Quaternion.LookRotation(dir);
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, rot);   // 부모 설정 없음

        /* -------- 4. P2P 동기화 -------- */
        P2PMessageSender.SendMessage(
            ObjectSpawnMessageBuilder.Build(spawnPos, rot, myPlayerNumber));

        /* -------- 5. 지속 시간 & 쿨타임 -------- */
        yield return new WaitForSeconds(ability.AttackHitboxDuration);
        Destroy(hitbox);

        yield return new WaitForSeconds(ability.AttackCooldown);
        canAttack = true;
    }
}