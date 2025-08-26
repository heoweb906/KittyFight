using UnityEngine;

public delegate void TickHandler(float dt);
public delegate void ModifyCooldownHandler(SkillType slot, ref float seconds);

public struct DashParams { public float distance; public float speed; }
public delegate void DashWillExecuteHandler(ref DashParams p);

// �̺�Ʈ ���
[DisallowMultipleComponent]
public class AbilityEvents : MonoBehaviour
{
    public event TickHandler OnTick;                         // �� ������
    public event ModifyCooldownHandler OnModifyCooldown;     // ��ٿ� ��� ����
    public event DashWillExecuteHandler OnDashWillExecute;   // �뽬 ���� ����

    public void EmitTick(float dt) => OnTick?.Invoke(dt);

    public void EmitModifyCooldown(SkillType slot, ref float seconds)
        => OnModifyCooldown?.Invoke(slot, ref seconds);

    public void EmitDashWillExecute(ref DashParams p)
        => OnDashWillExecute?.Invoke(ref p);
}