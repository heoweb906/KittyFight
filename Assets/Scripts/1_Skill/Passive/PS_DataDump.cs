using UnityEngine;

public class PS_DataDump : Passive
{
    [Header("쿨타임 초기화 주기")]
    [Tooltip("몇 초마다 한 번씩 모든 스킬 쿨타임을 초기화할지")]
    public float interval = 15f;
    private float timer = 0f;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        e.OnRoundStart += OnRoundStart;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnRoundStart -= OnRoundStart;
    }

    private void OnRoundStart(int roundIndex)
    {
        timer = 0f;
    }

    private void OnTick(float dt)
    {
        if (ability == null) return;
        if (interval <= 0f) return;

        timer += dt;
        if (timer >= interval)
        {
            timer -= interval;
            ability.ResetAllCooldowns();
        }
    }
}