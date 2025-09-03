using UnityEngine;

public class PS_Kickstart : Passive
{
    [Header("Kickstart Settings")]
    public float penaltyPerUse = 5.0f;   // ��ų ��� �� +5��
    public float reducePerJump = 0.6f;   // ������ ������ -0.6��(���� �� ��Ÿ�ӿ� ��� ����)

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnModifyCooldown += OnModifyCooldown; // ��ų ��Ÿ�� ���� ���� +5s
        e.OnJump += OnJump;                     // ����/������ �� ���� �� ��Ÿ�� -0.6s
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;
        e.OnJump -= OnJump;
    }

    void OnModifyCooldown(SkillType slot, ref float seconds)
    {
        // ��� ��ų�� +5�� (�ʿ� �� Ư�� ���Ը� ���͸� ����)
        seconds += Mathf.Max(0f, penaltyPerUse);
    }

    void OnJump()
    {
        // ���� ���� ��� ��Ÿ���� ��� 0.6�� ����
        ability?.ReduceAllCooldowns(Mathf.Max(0f, reducePerJump));
    }
}