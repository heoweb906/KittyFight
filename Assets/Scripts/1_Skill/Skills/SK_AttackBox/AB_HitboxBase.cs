using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using RayFire;

/// <summary>
/// ��� ���� ��Ʈ�ڽ�/����ü�� �������� ���� ����/����/ģ������/���� üũ ����
/// </summary>
public abstract class AB_HitboxBase : MonoBehaviour
{
    [Header("���� ������")]
    [Tooltip("������ Ability(������)")]
    protected PlayerAbility ownerAbility;

    [Tooltip("�� ���� �°� ����")]
    [SerializeField] protected bool singleHit = true;

    [Tooltip("����(��). 0 �����̸� �ڵ� �ı����� ����")]
    [SerializeField] public float lifeTime = 0.2f;

    [Header("ȯ�� �浹 ����")]
    [Tooltip("ȯ������ ������ ���̾�(��/�ٴ� ��)")]
    [SerializeField] protected LayerMask environmentMask;

    // �̹� ���� ���(�ߺ� ��Ʈ ����)
    private readonly HashSet<PlayerHealth> _hitOnce = new HashSet<PlayerHealth>();


    protected virtual void Awake()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);

        StartEffect();
    }

    /// <summary>
    /// ���� ���� ������ ���� (�ʼ�)
    /// </summary>
    public virtual void Init(PlayerAbility owner)
    {
        ownerAbility = owner;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        TryApplyHit(other);

        if (IsEnvironment(other.gameObject.layer))
            OnEnvironmentHit(other);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollision�� �۵���");

        var col = collision.collider;

        // 1) �÷��̾� �ǰ� �õ�
        if (TryApplyHit(col)) return;

        Debug.Log("������� �� �۵��ϰ� ����");

        // 2) ȯ�� �浹 ó��
        if (IsEnvironment(col.gameObject.layer))
            OnEnvironmentHit(col);
    }

    private bool TryApplyHit(Collider other)
    {
        // ������ �̼����̸� ���� �Ұ�
        if (ownerAbility == null) return false;

        // �ǰ��� Ž��(�ڽ� �ݶ��̴� ����)
        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        var victimAbility = other.GetComponentInParent<PlayerAbility>();

        if (victimHealth == null || victimAbility == null) return false;

        int ownerPN = ownerAbility.playerNumber;
        int victimPN = victimAbility.playerNumber;

        // �Ʊ�/�ڱ� �ڽ� ����
        if (victimPN == ownerPN) return false;

        // �� Ŭ���̾�Ʈ�� "���� �����(=��)"�� ���� ó�� (���� �Ͽ�ȭ)
        if (victimPN != MatchResultStore.myPlayerNumber) return false;

        // �ߺ� ��Ʈ ����
        if (singleHit && _hitOnce.Contains(victimHealth)) return false;
        _hitOnce.Add(victimHealth);

        // ���� ȿ�� ����
        ApplyEffects(victimHealth, other);
        return true;
    }

    protected bool IsEnvironment(int otherLayer)
    {
        return (environmentMask.value & (1 << otherLayer)) != 0;
    }


    protected virtual void StartEffect() { }





    public void Explode(float range, float force)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider col in colliders)
        {
            FragmentPiece piece = col.GetComponent<FragmentPiece>();

            if (piece != null)
            {
                Debug.Log("���� �߰�");


                piece.OnThisPiece();
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (col.transform.position - transform.position).normalized;
                    rb.AddForce(direction * force, ForceMode.Impulse);
                }
            }
        }
    }


    /// <summary>
    /// ȯ��(��/�ٴ� ��)���� ���� �� ����. �⺻�� �ƹ��͵� �� ��.
    /// ���Ÿ� ����ü ��� �ʿ� �� �������̵�.
    /// </summary>
    protected virtual void OnEnvironmentHit(Collider other) { }

    /// <summary>
    /// ���� ȿ�� ������: �Ļ� Ŭ�������� ȿ���� ����
    /// </summary>
    protected abstract void ApplyEffects(PlayerHealth victim, Collider victimCollider);
}