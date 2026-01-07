using UnityEngine;

public class PS_RazorTalons : Passive
{
    public override int PassiveId => 106;

    [Header("수치 설정")]
    [Tooltip("히트박스 크기 증가율 (0.4 = 40%)")]
    public float sizeIncreaseRate = 0.4f;

    [Tooltip("스킬 사거리(AimRange) 고정 증가값")]
    public float bonusAimRange = 0.5f; // 0.5 더하기

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    // 1. 장착 시: 0.5 더하기
    public override void OnEquip(PlayerAbility a)
    {
        base.OnEquip(a);

        var meleeSkill = a.GetComponentInChildren<SK_MeleeAttack>();
        if (meleeSkill != null)
        {
            meleeSkill.aimRange += bonusAimRange;
        }
    }

    // 2. 해제 시: 0.5 빼기 (원상복구)
    public override void OnUnequip()
    {
        if (ability != null)
        {
            var meleeSkill = ability.GetComponentInChildren<SK_MeleeAttack>();
            if (meleeSkill != null)
            {
                meleeSkill.aimRange -= bonusAimRange;
            }
        }
        base.OnUnequip();
    }

    // --- 기존 히트박스 크기 로직 유지 ---

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnMeleeHitboxSpawned += OnMeleeHitboxSpawned;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnMeleeHitboxSpawned -= OnMeleeHitboxSpawned;
        base.Unsubscribe(e);
    }

    private void OnMeleeHitboxSpawned(AB_MeleeHitbox hb)
    {
        if (hb == null) return;

        float factor = 1f + sizeIncreaseRate;

        var tr = hb.transform;
        tr.localScale *= factor;

        if (effectPrefab != null)
        {
            Instantiate(
               effectPrefab,
               hb.transform.position,
               hb.transform.rotation * Quaternion.Euler(-120f, -90f, 90f)
           );
        }
    }
}