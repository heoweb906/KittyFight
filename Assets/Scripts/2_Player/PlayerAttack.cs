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

        // 1) 화면 좌표 → 월드 좌표 변환
        Vector3 mp = Input.mousePosition;
        // 카메라와 플레이어가 z축(깊이) 상으로 떨어진 거리
        mp.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mp);
        // y 높이를 플레이어와 동일하게 고정
        //worldClick.y = transform.position.y;

        // 2) 방향 계산 & 정규화
        Vector3 dir = worldClick - transform.position;
        if (dir.sqrMagnitude < 0.001f)
        {
            // 클릭 위치가 내 위치와 거의 같으면, 전방으로
            dir = transform.forward;
        }
        dir.Normalize();

        // 3) 항상 최대 사거리만큼 떨어진 지점에 생성
        Vector3 spawnPos = transform.position + dir * maxAttackRange;
        Quaternion rot = Quaternion.LookRotation(dir);

        // 4) 히트박스 생성
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, rot);

        // 5) P2P 동기화
        P2PMessageSender.SendMessage(
            ObjectSpawnMessageBuilder.Build(spawnPos, rot, myPlayerNumber));

        // 6) 지속시간 & 쿨타임
        yield return new WaitForSeconds(ability.AttackHitboxDuration);
        Destroy(hitbox);

        yield return new WaitForSeconds(ability.AttackCooldown);
        canAttack = true;
    }
}