using UnityEngine;

public class SK_MeleeAttack : Skill
{
    public AbilityEvents events;

    [Header("Effects")]
    [SerializeField] private GameObject meleeEffectPrefab;   // �������� ����Ʈ

    [Header("ī�޶�")]
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

        // ������(������)�� �⺻ ���� ������ �б�
        int damage = 0;
        var protoHB = objSkillEntity.GetComponent<AB_MeleeHitbox>();
        if (protoHB != null) damage = protoHB.damage;

        // TalonsEdge �� �нú갡 ������ ���⼭ �����/����
        events?.EmitMeleeDamageInt(ref damage);

        GameObject hitbox = Instantiate(objSkillEntity, spawnPos, rot);

        // ���� ���� �������� ������ ��Ʈ�ڽ��� ����
        var hb = hitbox.GetComponent<AB_MeleeHitbox>();
        if (hb != null) hb.damage = damage;

        // ���� Init: ������ Ability ���� (playerNumber�� ���ο��� ���õ�)
        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);


        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCamera(shakeAmount, 0.2f);
        }
    }
}