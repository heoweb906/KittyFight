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


// �̺�Ʈ ���
[DisallowMultipleComponent]
public class AbilityEvents : MonoBehaviour
{
    public event TickHandler OnTick;                        // �� ������
    public event JumpHandler OnJump;                        // ������ ��
    public event ModifyCooldownHandler OnModifyCooldown;    // ��ٿ� ��� ����
    public event CooldownFinalizedHandler OnCooldownFinalized;
    public event DashWillExecuteHandler OnDashWillExecute;  // �뽬 ���� ����
    public event BeforeDealDamageHandler OnBeforeDealDamage; // ������ ��� ��
    public event MeleeDamageIntHandler OnMeleeDamageInt;    // ���������� ����
    public event RoundHandler OnRoundStart;                 // ���� ����

    public void EmitTick(float dt) => OnTick?.Invoke(dt);

    public void EmitJump() => OnJump?.Invoke();

    public void EmitModifyCooldown(SkillType slot, ref float seconds)
        => OnModifyCooldown?.Invoke(slot, ref seconds);

    public void EmitCooldownFinalized(SkillType slot, float finalSeconds)
        => OnCooldownFinalized?.Invoke(slot, finalSeconds);

    public void EmitDashWillExecute(ref DashParams p)
        => OnDashWillExecute?.Invoke(ref p);

    public void EmitBeforeDealDamage(ref int dmg, GameObject victim)
        => OnBeforeDealDamage?.Invoke(ref dmg, victim);

    public void EmitMeleeDamageInt(ref int damage) => OnMeleeDamageInt?.Invoke(ref damage);

    public void EmitRoundStart(int roundIndex) => OnRoundStart?.Invoke(roundIndex);

}