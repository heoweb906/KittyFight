using UnityEngine;

public class SK_RangedAttack : Skill
{
    public float projectileSpeed = 10f;

    [Header("Visuals")]
    [SerializeField] private Material p1Material;
    [SerializeField] private Material p2Material;

    [Header("ī�޶�")]
    public float shakeAmount = 0.06f;

    private bool flag = false;
    private Vector3 pendingPos;
    private Vector3 pendingDir;

    private void Awake()
    {
        coolTime = 3.0f;
        aimRange = 1.0f;
    }

    private void Update()
    {
        if (!flag) return;

        Vector3 spawnPos = pendingPos;
        Quaternion rot = Quaternion.LookRotation(pendingDir);

        if (objSkillEntity == null) return;

        GameObject proj = Instantiate(objSkillEntity, spawnPos, rot);

        // ���� Init: ������ Ability ���� (playerNumber�� ���ο��� ���õ�)
        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        ApplyPerPlayerMaterial(proj);

        // ��/�ӵ� �ο�(���� ������ ������)
        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = pendingDir * projectileSpeed;


        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCamera(shakeAmount, 0.2f);
        }

        flag = false;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        pendingPos = origin;
        pendingDir = direction;
        flag = true;
    }

    private void ApplyPerPlayerMaterial(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            // ���� ���׸����� �����ؼ� ���� �����
            Material newMat = new Material(r.sharedMaterial);

            // �ƿ����� ���� ����
            Color outlineColor = (playerAbility.playerNumber == 1) ? Color.red : Color.blue;
            newMat.SetColor("_OutlineColorVertex", outlineColor);

            // �� ���׸��� ����
            r.material = newMat;
        }
    }
}