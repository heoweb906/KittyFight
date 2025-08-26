using UnityEngine;

public delegate void TickHandler(float dt);
public delegate void ModifyCooldownHandler(SkillType slot, ref float seconds);

public struct DashParams { public float distance; public float speed; }
public delegate void DashWillExecuteHandler(ref DashParams p);

// 이벤트 허브
[DisallowMultipleComponent]
public class AbilityEvents : MonoBehaviour
{
    public event TickHandler OnTick;                         // 매 프레임
    public event ModifyCooldownHandler OnModifyCooldown;     // 쿨다운 계산 직전
    public event DashWillExecuteHandler OnDashWillExecute;   // 대쉬 실행 직전

    public void EmitTick(float dt) => OnTick?.Invoke(dt);

    public void EmitModifyCooldown(SkillType slot, ref float seconds)
        => OnModifyCooldown?.Invoke(slot, ref seconds);

    public void EmitDashWillExecute(ref DashParams p)
        => OnDashWillExecute?.Invoke(ref p);
}