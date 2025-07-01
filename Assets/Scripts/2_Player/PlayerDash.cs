// PlayerDash.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAbility))]
public class PlayerDash : MonoBehaviour
{
    public float dashDistance = 3f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 1f;
    public int myPlayerNumber;

    private Rigidbody rb;
    private bool canDash = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TryDash()
    {
        if (canDash)
            StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;

        // ȭ�� ��ǥ �� ���� ��ǥ ��ȯ (z�ุ ī�޶�-�÷��̾� �Ÿ�)
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mp);

        // Ŀ�� �������� ���� ��� (3D �밢�� ����)
        Vector3 dir = (worldClick - transform.position).normalized;
        if (dir == Vector3.zero)
            dir = transform.forward;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dir * dashDistance;

        // �뽬 �� �߷� ����
        bool origGravity = rb.useGravity;
        rb.useGravity = false;

        // Lerp �̵�
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, elapsed / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.MovePosition(targetPos);

        // �뽬 �Ϸ� �� �߷� ����
        rb.useGravity = origGravity;

        // ��Ÿ��
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}