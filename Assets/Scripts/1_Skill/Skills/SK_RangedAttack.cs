using UnityEngine;

public class SK_RangedAttack : Skill
{
    public float projectileSpeed = 10f;

    [Header("Visuals")]
    [SerializeField] private Material p1Material;
    [SerializeField] private Material p2Material;

    private void Awake()
    {
        coolTime = 3.0f;
        aimRange = 2.5f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Vector3 spawnPos = origin;
        Quaternion rot = Quaternion.LookRotation(direction);

        if (objSkillEntity == null) return;

        GameObject proj = Instantiate(objSkillEntity, spawnPos, rot);

        // ���� Init: ������ Ability ���� (playerNumber�� ���ο��� ���õ�)
        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        ApplyPerPlayerMaterial(proj);

        // ��/�ӵ� �ο�(���� ������ ������)
        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = direction * projectileSpeed;
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