using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] protected float lifeTime = 1.0f;

    // �̹� ���� ���(�ߺ� ��Ʈ ����)
    private readonly HashSet<PlayerHealth> _hitOnce = new HashSet<PlayerHealth>();

    protected virtual void Awake()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// ���� ���� ������ ���� (�ʼ�)
    /// </summary>
    public virtual void Init(PlayerAbility owner)
    {
        ownerAbility = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �ǰ��� ������Ʈ ȹ�� (�ڽ� �ݶ��̴� ����)
        var victimHealth = other.GetComponentInParent<PlayerHealth>();
        if (victimHealth == null) return;

        var victimAbility = other.GetComponentInParent<PlayerAbility>();
        if (victimAbility == null) return;

        // ������ �̼����̸� ���� �Ұ� �� ���� Ż��
        if (ownerAbility == null) return;

        int ownerPN = ownerAbility.playerNumber;
        int victimPN = victimAbility.playerNumber;

        // �Ʊ�/�ڱ� �ڽ� ����
        if (victimPN == ownerPN) return;

        // �� Ŭ���̾�Ʈ�� "���� �����(=��)"�� ���� ó�� (���� �Ͽ�ȭ)
        if (victimPN != MatchResultStore.myPlayerNumber) return;

        // �ߺ� ��Ʈ ����
        if (singleHit && _hitOnce.Contains(victimHealth)) return;
        _hitOnce.Add(victimHealth);

        // ���� ȿ�� ����(������/�˹�/���� ��)
        ApplyEffects(victimHealth, other);
    }

    /// <summary>
    /// ���� ȿ�� ������: �Ļ� Ŭ�������� ȿ���� ����
    /// </summary>
    protected abstract void ApplyEffects(PlayerHealth victim, Collider victimCollider);
}