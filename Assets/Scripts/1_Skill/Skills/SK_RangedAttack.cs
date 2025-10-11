using UnityEngine;

public class SK_RangedAttack : Skill
{
    public float projectileSpeed = 10f;

    [Header("Visuals")]
    [SerializeField] private Material p1Material;
    [SerializeField] private Material p2Material;

    [Header("카메라")]
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

        // 공통 Init: 소유자 Ability 주입 (playerNumber도 내부에서 세팅됨)
        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        ApplyPerPlayerMaterial(proj);

        // 힘/속도 부여(그저 앞으로 날리기)
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
            // 기존 머테리얼을 복사해서 새로 만들기
            Material newMat = new Material(r.sharedMaterial);

            // 아웃라인 색상 설정
            Color outlineColor = (playerAbility.playerNumber == 1) ? Color.red : Color.blue;
            newMat.SetColor("_OutlineColorVertex", outlineColor);

            // 새 머테리얼 적용
            r.material = newMat;
        }
    }
}