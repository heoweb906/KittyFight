using UnityEngine;

public delegate void TickHandler(float dt);
public delegate void JumpHandler();
public delegate void ModifyCooldownHandler(SkillType slot, ref float seconds);
public delegate void CooldownFinalizedHandler(SkillType slot, float finalSeconds);
public delegate void SkillExecutedHandler(SkillType slot);
public delegate void MeleeDamageIntHandler(ref int damage);
public delegate void MeleeHitboxSpawnedHandler(AB_MeleeHitbox hb);
public delegate void HitboxSpawnedHandler(AB_HitboxBase hb);
public delegate void RoundHandler(int roundIndex);
public delegate void BeforeDealDamageHandler(ref int dmg, GameObject victim);
public delegate void BeforeTakeDamageHandler(ref int dmg, GameObject attacker);
public delegate void DealtDamageHandler();

public struct DashParams { public float distance; public float speed; public Vector3 direction; }
public delegate void DashWillExecuteHandler(ref DashParams p);
public delegate void DashFinishedHandler(Vector3 start, Vector3 end);


// 이벤트 허브
[DisallowMultipleComponent]
public class AbilityEvents : MonoBehaviour
{
    public event TickHandler OnTick;                        // 매 프레임 (101, 103, 113, 118, 119)
    public event JumpHandler OnJump;                        // 점프할 때 (109, 110)
    public event ModifyCooldownHandler OnModifyCooldown;    // 쿨다운 계산 직전 (104, 109, 112, 114)
    public event CooldownFinalizedHandler OnCooldownFinalized; // (103)
    public event SkillExecutedHandler OnSkillExecuted;  // 어떤 스킬이라도 사용 시 (119)
    public event DashWillExecuteHandler OnDashWillExecute;  // 대쉬 실행 직전 (103)
    public event DashFinishedHandler OnDashFinished; // 대쉬 종료
    public event BeforeDealDamageHandler OnBeforeDealDamage; // 데미지 계산 전 공격자용 (133)
    public event BeforeTakeDamageHandler OnBeforeTakeDamage; // 데미지 계산 전 수비자용 (102, 118, 120)
    public event DealtDamageHandler OnDealtDamage;          // 상대한테 피해를 줄 때 나 (107)
    public event MeleeDamageIntHandler OnMeleeDamageInt;    // 근접데미지 계산시 (108)
    public event MeleeHitboxSpawnedHandler OnMeleeHitboxSpawned; // 근접공격 범위 (106)
    public event HitboxSpawnedHandler OnHitboxSpawned; // 원거리 공격 (114)
    public event RoundHandler OnRoundStart;                 // 라운드 시작 (101, 108, 118)

    public void EmitTick(float dt) => OnTick?.Invoke(dt);

    public void EmitJump() => OnJump?.Invoke();

    public void EmitModifyCooldown(SkillType slot, ref float seconds)
        => OnModifyCooldown?.Invoke(slot, ref seconds);

    public void EmitCooldownFinalized(SkillType slot, float finalSeconds)
        => OnCooldownFinalized?.Invoke(slot, finalSeconds);

    public void EmitSkillExecuted(SkillType slot)
        => OnSkillExecuted?.Invoke(slot);

    public void EmitDashWillExecute(ref DashParams p)
        => OnDashWillExecute?.Invoke(ref p);

    public void EmitDashFinished(Vector3 start, Vector3 end)
        => OnDashFinished?.Invoke(start, end);

    public void EmitBeforeDealDamage(ref int dmg, GameObject victim)
        => OnBeforeDealDamage?.Invoke(ref dmg, victim);

    public void EmitBeforeTakeDamage(ref int dmg, GameObject attacker)
        => OnBeforeTakeDamage?.Invoke(ref dmg, attacker);

    public void EmitDealtDamage()
        => OnDealtDamage?.Invoke();

    public void EmitMeleeDamageInt(ref int damage) => OnMeleeDamageInt?.Invoke(ref damage);

    public void EmitMeleeHitboxSpawned(AB_MeleeHitbox hb)
        => OnMeleeHitboxSpawned?.Invoke(hb);

    public void EmitHitboxSpawned(AB_HitboxBase hb)
    => OnHitboxSpawned?.Invoke(hb);

    public void EmitRoundStart(int roundIndex) => OnRoundStart?.Invoke(roundIndex);

}