using UnityEngine;
using UnityEngine.EventSystems;
public class AB_HoofOut : AB_HitboxBase
{
    private BoxCollider boxColiider;

    [Header("����/����")]
    public int damage = 40;
    public float knockbackForce = 10f;
    public float disableControlSeconds = 0.5f;

    [Header("�˹� ����")]
    public float lateralFactor = 1f; // ��/�� ���� ����ġ
    public float upwardFactor = 1f; // ���� ���� ����ġ
    // SK���� �Ѱ��ִ� ��/�� ��ȣ(+1: ������, -1: ����)
    private float lateralSign = 1f;


    [Header("Effect")]
    public GameObject obj_Effeect;
    public GameObject obj_Hoof;
    private float moveSpeed = 0.3f;
    private bool isMoving = false;


    private void Update()
    {
        if (isMoving)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }


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
            StartCoroutine(ReenableAfter(controller, disableControlSeconds));
        }
        // ������
        victim.TakeDamage(damage, ownerAbility, transform.position);
    }
    private System.Collections.IEnumerator ReenableAfter(MonoBehaviour m, float delay)
    {
        if (m != null) m.enabled = false;
        yield return new WaitForSeconds(delay);
        if (m != null) m.enabled = true;
    }
    protected override void StartEffect()
    {
        if (obj_Effeect != null)
        {
            GameObject effect = Instantiate(obj_Effeect, transform);
            effect.transform.localPosition = new Vector3(-0.004f, 0f, -0.465f);
            effect.transform.localRotation = Quaternion.Euler(-28.621f, 179.858f, -89.627f);
            effect.transform.localScale = new Vector3(1.001007f, 1.001007f, 1.001007f);
            effect.transform.SetParent(null);
        }
        isMoving = true;

    }


    private void OnDestroy()
    {
        obj_Hoof.transform.SetParent(null);
        ProbPiece piece = obj_Hoof.GetComponent<ProbPiece>();
        piece.OnThisPiece();
    }

}