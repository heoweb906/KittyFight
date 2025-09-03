using UnityEngine;

public class PS_TalonsEdge : Passive
{
    [Header("Talons' Edge")]
    public int basePoints = 5;    // ���� ���۸��� �� ������
    public int addPerUse = 2;     // ���� "���"�� ������ +0.2

    private int currentPoints;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnRoundStart += HandleRoundStart;
        e.OnMeleeDamageInt += HandleMeleeDamageInt;

        // ���� ���� �ʱ�ȭ
        HandleRoundStart(0);
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnRoundStart -= HandleRoundStart;
        e.OnMeleeDamageInt -= HandleMeleeDamageInt;
    }

    void HandleRoundStart(int _)
    {
        currentPoints = basePoints;
    }

    void HandleMeleeDamageInt(ref int damage)
    {
        damage = currentPoints;

        currentPoints += addPerUse;
    }
}