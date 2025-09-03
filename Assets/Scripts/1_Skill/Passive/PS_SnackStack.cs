using UnityEngine;

public class PS_SnackStack : Passive
{
    [Header("Snack & Stack")]
    public int initialMaxHP = 50;   // 장착 즉시 5
    public int growPerRound = 20;    // 라운드 시작마다 +2
    public bool keepRatioOnChange = false; // 보통 false: 힐은 안 됨

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