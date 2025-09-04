using UnityEngine;

public class SK_MeleeAttack : Skill
{
    public AbilityEvents events;

    [Header("Effects")]
    [SerializeField] private GameObject meleeEffectPrefab;   // 근접공격 이펙트

    [Header("카메라")]
    public float shakeAmount = 0.24f;

    private void Awake()
    {
        coolTime = 3.0f;
        aimRange = 1.0f;
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


        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCamera(shakeAmount, 0.2f);
        }
    }
}