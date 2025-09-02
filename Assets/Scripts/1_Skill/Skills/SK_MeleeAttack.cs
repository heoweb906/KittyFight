using UnityEngine;

public class SK_MeleeAttack : Skill
{
    [Header("Visuals")]
    [SerializeField] private Material p1Material;
    [SerializeField] private Material p2Material;

    public AbilityEvents events;
    private void Awake()
    {
        coolTime = 3.0f;
        aimRange = 2.5f;
        if (!events && playerAbility) events = playerAbility.events;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Vector3 spawnPos = origin;
        Quaternion rot = Quaternion.LookRotation(direction);

        if (objSkillEntity == null) return;

        // 프리팹(프로토)의 기본 정수 데미지 읽기
        int damage = 0;
        var protoHB = objSkillEntity.GetComponent<AB_MeleeHitbox>();
        if (protoHB != null) damage = protoHB.damage;

        // TalonsEdge 등 패시브가 있으면 여기서 덮어쓰기/보정
        events?.EmitMeleeDamageInt(ref damage);

        GameObject hitbox = Instantiate(objSkillEntity, spawnPos, rot);

        // 최종 정수 데미지를 스폰된 히트박스에 주입
        var hb = hitbox.GetComponent<AB_MeleeHitbox>();
        if (hb != null) hb.damage = damage;

        // 공통 Init: 소유자 Ability 주입 (playerNumber도 내부에서 세팅됨)
        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        ApplyPerPlayerMaterial(hitbox);
    }

    private void ApplyPerPlayerMaterial(GameObject go)
    {
        var mat = (playerAbility.playerNumber == 1) ? p1Material : p2Material;
        if (mat == null) return;

        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            r.sharedMaterial = mat;
        }
    }
}