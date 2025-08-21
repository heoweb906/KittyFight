using UnityEngine;

public class AB_HoofOut : AB_HitboxBase
{
    [Header("����/����")]
    public int damage = 40;
    public float knockbackForce = 10f;
    public float disableControlSeconds = 0.5f;

    [Header("�˹� ����")]
    public float lateralFactor = 1f; // ��/�� ���� ����ġ
    public float upwardFactor = 1f; // ���� ���� ����ġ

    // SK���� �Ѱ��ִ� ��/�� ��ȣ(+1: ������, -1: ����)
    private float lateralSign = 1f;

    public void SetLateralSign(float sign)
    {
        lateralSign = sign >= 0f ? 1f : -1f;
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // �˹�: ���� �� Ȯ���� ��/�� ��ȣ�� ���(���� ��ġ ���� ����)
        var rb = victim.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 kbDir = (Vector3.right * lateralSign * lateralFactor + Vector3.up * upwardFactor).normalized;

            rb.velocity = Vector3.zero;
            rb.AddForce(kbDir * knockbackForce, ForceMode.Impulse);
        }

        // ��Ʈ�� ���� (�ǰ��� �ʿ����� �����: AB_HitboxBase ���� ��Ģ)
        var controller = victim.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
            StartCoroutine(ReenableAfter(controller, disableControlSeconds));
        }

        // ������
        victim.TakeDamage(damage);
    }

    private System.Collections.IEnumerator ReenableAfter(MonoBehaviour m, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m != null) m.enabled = true;
    }
}