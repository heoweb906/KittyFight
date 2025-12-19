using UnityEngine;
using System.Collections;

public class SK_RangedAttack : Skill
{
    public float projectileSpeed = 10f;

    [Header("카메라")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Bind(PlayerAbility ability)
    {
        base.Bind(ability);
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

        // 중력 끌수 있는 TailMail 용
        if (ab != null && playerAbility != null && playerAbility.events != null)
        {
            playerAbility.events.EmitHitboxSpawned(ab);
        }

        if (playerAbility.playerNumber == 2)
        {
            AB_Ranged range = proj.GetComponent<AB_Ranged>();
            range.ChangeMaterial();
        }


        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 2);
        StartCoroutine(ResetAttackAnimState());

        ApplyPerPlayerMaterial(proj);

        // 힘/속도 부여(그저 앞으로 날리기)
        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = direction * projectileSpeed;

        // 훈련장용
        if (playerAbility.playerNumber == 0)
        {
            var cm = FindObjectOfType<CameraManager>();
            cm?.ShakeCameraPunch(shakeAmount, shakeAmount, direction);
        }
        else if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeAmount, direction);
        }
        else
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeAmount * 0.5f, direction);
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
            Color outlineColor = Color.red;
            if(playerAbility.playerNumber == 2) outlineColor = Color.blue;

            newMat.SetColor("_OutlineColorVertex", outlineColor);

            // 새 머테리얼 적용
            r.material = newMat;
        }
    }

    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}