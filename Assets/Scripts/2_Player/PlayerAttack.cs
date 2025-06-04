using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerAbility))]
public class PlayerAttack : MonoBehaviour
{
    public GameObject hitboxPrefab;

    public float minSpawnDistance = 0.5f;   // �ʹ� ����� ��� ����
    public float maxAttackRange = 3.5f;   // �ִ� ��Ÿ�

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

        /* -------- 1. ���콺 �� ���� ���� ��� -------- */
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector3 dir = mouseWorld - transform.position;
        dir.y = 0f;                                // ���� ���⸸ ���
        if (dir == Vector3.zero) dir = transform.forward; // ���콺�� ��Ȯ�� ����� �ӽ÷� ���� ���
        dir.Normalize();

        /* -------- 2. ��Ÿ� Ŭ���� -------- */
        float spawnDistance = Mathf.Clamp(Vector3.Distance(transform.position, mouseWorld),
                                          minSpawnDistance,
                                          maxAttackRange);

        /* -------- 3. ��Ʈ�ڽ� ���� (�÷��̾�� ȸ�� X) -------- */
        Vector3 spawnPos = transform.position + dir * spawnDistance;
        Quaternion rot = Quaternion.LookRotation(dir);
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, rot);   // �θ� ���� ����

        /* -------- 4. P2P ����ȭ -------- */
        P2PMessageSender.SendMessage(
            ObjectSpawnMessageBuilder.Build(spawnPos, rot, myPlayerNumber));

        /* -------- 5. ���� �ð� & ��Ÿ�� -------- */
        yield return new WaitForSeconds(ability.AttackHitboxDuration);
        Destroy(hitbox);

        yield return new WaitForSeconds(ability.AttackCooldown);
        canAttack = true;
    }
}