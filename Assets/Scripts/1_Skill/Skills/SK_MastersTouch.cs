using UnityEngine;
using System.Collections;
public class SK_MastersTouch : Skill
{
    [SerializeField] private GameObject effectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 6);
        StartCoroutine(ResetAttackAnimState());


        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.Euler(-90f, 0, 0)
        );

        var ability = playerAbility;

        SkillType[] slots =
        {
            SkillType.Melee,
            SkillType.Ranged,
            SkillType.Dash,
            SkillType.Skill1,
            SkillType.Skill2
        };

        foreach (var slot in slots)
        {
            if (slot == assignedSlot) continue;
            ability.CancelCooldown(slot);
        }


        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
    }

    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}