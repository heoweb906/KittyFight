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

        // 1) ȭ�� ��ǥ �� ���� ��ǥ ��ȯ
        Vector3 mp = Input.mousePosition;
        // ī�޶�� �÷��̾ z��(����) ������ ������ �Ÿ�
        mp.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mp);
        // y ���̸� �÷��̾�� �����ϰ� ����
        //worldClick.y = transform.position.y;

        // 2) ���� ��� & ����ȭ
        Vector3 dir = worldClick - transform.position;
        if (dir.sqrMagnitude < 0.001f)
        {
            // Ŭ�� ��ġ�� �� ��ġ�� ���� ������, ��������
            dir = transform.forward;
        }
        dir.Normalize();

        // 3) �׻� �ִ� ��Ÿ���ŭ ������ ������ ����
        Vector3 spawnPos = transform.position + dir * maxAttackRange;
        Quaternion rot = Quaternion.LookRotation(dir);

        // 4) ��Ʈ�ڽ� ����
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, rot);

        // 5) P2P ����ȭ
        P2PMessageSender.SendMessage(
            ObjectSpawnMessageBuilder.Build(spawnPos, rot, myPlayerNumber));

        // 6) ���ӽð� & ��Ÿ��
        yield return new WaitForSeconds(ability.AttackHitboxDuration);
        Destroy(hitbox);

        yield return new WaitForSeconds(ability.AttackCooldown);
        canAttack = true;
    }
}