using UnityEngine;

public class PS_ChronoChaos : Passive
{
    public override int PassiveId => 139;

    [Header("랜덤 쿨타임 보정 범위")]
    [Tooltip("적용 가능한 쿨타임 보정값 목록 (0은 포함하지 말 것)")]
    public int[] offsets = new int[] { -3, -2, -1, 1, 2, 3, 4, 5 };

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;
    public float targetWorldSize;


    private VFX_139_ChronoChaos vfx;

    protected override void Subscribe(AbilityEvents e)
    {
        if (effectPrefab != null)
        {
            // 1. 생성 (위치, 회전, 부모 설정)
            var go = Instantiate(
                effectPrefab,
                transform.position,
                Quaternion.Euler(new Vector3(0f, 0f, 0f)), // 여기에 회전값 적용
                transform
            );

            // 2. 크기 보정 (부모 크기 무시하고 월드 크기 고정)
            // 현재 부모의 월드 스케일로 나누어 주어, 자식은 항상 targetWorldSize 크기를 유지
            Vector3 parentScale = transform.lossyScale;

            // 0으로 나누는 것 방지 (안전장치)
            if (parentScale.x != 0 && parentScale.y != 0 && parentScale.z != 0)
            {
                go.transform.localScale = new Vector3(
                    targetWorldSize / parentScale.x,
                    targetWorldSize / parentScale.y,
                    targetWorldSize / parentScale.z
                );
            }
            else
            {
                // 부모 스케일이 0일 경우 그냥 설정값 대입 (예외 처리)
                go.transform.localScale = Vector3.one * targetWorldSize;
            }

            // 3. 컴포넌트 가져오기
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
        if (type == SkillType.Dash || type == SkillType.Melee || type == SkillType.Ranged)
            return;

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
