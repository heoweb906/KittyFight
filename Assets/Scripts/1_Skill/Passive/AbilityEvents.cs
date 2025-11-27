using UnityEngine;

public delegate void TickHandler(float dt);
public delegate void JumpHandler();
public delegate void ModifyCooldownHandler(SkillType slot, ref float seconds);
public delegate void CooldownFinalizedHandler(SkillType slot, float finalSeconds);
public delegate void MeleeDamageIntHandler(ref int damage);
public delegate void RoundHandler(int roundIndex);
public delegate void BeforeDealDamageHandler(ref int dmg, GameObject victim);

public struct DashParams { public float distance; public float speed; }
public delegate void DashWillExecuteHandler(ref DashParams p);
public delegate void DashFinishedHandler(Vector3 start, Vector3 end);


// 이벤트 허브
[DisallowMultipleComponent]
public class AbilityEvents : MonoBehaviour
{
    public event TickHandler OnTick;                        // 매 프레임
    public event JumpHandler OnJump;                        // 점프할 때
    public event ModifyCooldownHandler OnModifyCooldown;    // 쿨다운 계산 직전
    public event CooldownFinalizedHandler OnCooldownFinalized;
    public event DashWillExecuteHandler OnDashWillExecute;  // 대쉬 실행 직전
    public event DashFinishedHandler OnDashFinished; // 대쉬 종료
    public event BeforeDealDamageHandler OnBeforeDealDamage; // 데미지 계산 전
    public event MeleeDamageIntHandler OnMeleeDamageInt;    // 근접데미지 계산시
    public event RoundHandler OnRoundStart;                 // 라운드 시작

    public void EmitTick(float dt) => OnTick?.Invoke(dt);

    public void EmitJump() => OnJump?.Invoke();

    public void EmitModifyCooldown(SkillType slot, ref float seconds)
        => OnModifyCooldown?.Invoke(slot, ref seconds);

    public void EmitCooldownFinalized(SkillType slot, float finalSeconds)
        => OnCooldownFinalized?.Invoke(slot, finalSeconds);

    public void EmitDashWillExecute(ref DashParams p)
        => OnDashWillExecute?.Invoke(ref p);

    public void EmitDashFinished(Vector3 start, Vector3 end)
        => OnDashFinished?.Invoke(start, end);

    public void EmitBeforeDealDamage(ref int dmg, GameObject victim)
        => OnBeforeDealDamage?.Invoke(ref dmg, victim);

    public void EmitMeleeDamageInt(ref int damage) => OnMeleeDamageInt?.Invoke(ref damage);

    public void EmitRoundStart(int roundIndex) => OnRoundStart?.Invoke(roundIndex);

}