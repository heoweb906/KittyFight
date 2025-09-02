using UnityEngine;

public class PS_SnackStack : Passive
{
    [Header("Snack & Stack")]
    public int initialMaxHP = 50;   // ���� ��� 5
    public int growPerRound = 20;    // ���� ���۸��� +2
    public bool keepRatioOnChange = false; // ���� false: ���� �� ��

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnRoundStart += OnRoundStart;

        var hp = ability.GetComponent<PlayerHealth>();
        if (!hp) return;

        hp.SetMaxHP(initialMaxHP, keepRatioOnChange);
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnRoundStart -= OnRoundStart;
    }

    void OnRoundStart(int roundIndex)
    {
        var hp = ability.GetComponent<PlayerHealth>();
        if (!hp) return;

        hp.AddMaxHP(growPerRound, keepRatioOnChange);
    }
}