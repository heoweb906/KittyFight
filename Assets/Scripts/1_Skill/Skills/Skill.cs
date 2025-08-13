using UnityEngine;

public interface ISKILL
{
    void Execute(Vector3 origin, Vector3 direction);
}

public abstract class Skill : MonoBehaviour, ISKILL
{
    [Header("�⺻ ����")]
    public PlayerAbility playerAbility;

    [Header("��ų ����(������)")]
    public float coolTime;   // Ability�� �����ϴ� ������
    public GameObject objSkillEntity;
    public Sprite skillIcon;

    protected SkillType assignedSlot;

    public void SetNewBasicValue(PlayerAbility ability) => playerAbility = ability;
    public void SetAssignedSlot(SkillType slot) => assignedSlot = slot;

    // ���� ���� ���� �߻�, �� ��ų�� �̰� ����
    public abstract void Execute(Vector3 origin, Vector3 direction);
}