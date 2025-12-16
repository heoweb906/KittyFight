using UnityEngine;

public class PS_ChronoChaos : Passive
{
    public override int PassiveId => 139;

    [Header("랜덤 쿨타임 보정 범위")]
    [Tooltip("적용 가능한 쿨타임 보정값 목록 (0은 포함하지 말 것)")]
    public int[] offsets = new int[] { -3, -2, -1, 1, 2, 3, 4, 5 };

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;
    private VFX_139_ChronoChaos vfx;

    protected override void Subscribe(AbilityEvents e)
    {
        if (effectPrefab != null)
        {
            var go = Instantiate(
                effectPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            vfx = go.GetComponent<VFX_139_ChronoChaos>();
        }
        e.OnModifyCooldown += OnModifyCooldown;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;
    }

    private void OnModifyCooldown(SkillType type, ref float duration)
    {
        if (!IsAuthority) return;
        if (duration <= 0f) return;
        if (offsets == null || offsets.Length == 0) return;

        int delta = offsets[Random.Range(0, offsets.Length)];
        if (delta == 0) return;

        duration = Mathf.Max(0f, duration + delta);

        vfx?.Play();
        SendProc(
            PassiveProcType.Value,
            pos: transform.position,
            dir: Vector3.up,
            i0: (int)type,
            f0: delta
        );
    }
    public override void RemoteExecute(PassiveProcMessage msg)
    {
        if (ability == null) return;

        var slot = (SkillType)msg.i0;
        float delta = msg.f0;
        ability.ApplyCooldownDelta(slot, delta);

        vfx?.Play();
    }
}
