using UnityEngine;

public class PS_ChronoChaos : Passive
{
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
        if (duration <= 0f) return;
        if (offsets == null || offsets.Length == 0) return;

        int index = Random.Range(0, offsets.Length);
        int delta = offsets[index];

        if (delta == 0) return;

        duration += delta;

        if (vfx != null) vfx.Play();
        if (duration < 0f) duration = 0f;
    }
}
