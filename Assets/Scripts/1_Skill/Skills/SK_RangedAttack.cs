using UnityEngine;

public class SK_RangedAttack : Skill
{
    public float projectileSpeed = 10f;

    [Header("Visuals")]
    [SerializeField] private Material p1Material;
    [SerializeField] private Material p2Material;

    [Header("카메라")]
    public float shakeAmount = 0.06f;

    private void Awake()
    {
        coolTime = 3.0f;
        aimRange = 1.0f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Vector3 spawnPos = origin;
        Quaternion rot = Quaternion.LookRotation(direction);

        if (objSkillEntity == null) return;

        GameObject proj = Instantiate(objSkillEntity, spawnPos, rot);

        // 공통 Init: 소유자 Ability 주입 (playerNumber도 내부에서 세팅됨)
        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        ApplyPerPlayerMaterial(proj);

        // 힘/속도 부여(그저 앞으로 날리기)
        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = direction * projectileSpeed;

        // 훈련장용
        if (playerAbility.playerNumber == 0)
        {
            var cm = FindObjectOfType<CameraManager>();
            cm?.ShakeCamera(shakeAmount, 0.2f);
        }

        else if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCamera(shakeAmount, 0.2f);
        }
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