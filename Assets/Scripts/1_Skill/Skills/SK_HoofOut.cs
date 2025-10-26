using UnityEngine;

public class SK_HoofOut : Skill
{
    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (objSkillEntity == null) return;

        Vector3 spawnPos = origin;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        GameObject hitbox = Instantiate(objSkillEntity, spawnPos, rot);

        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        // HoofOut ����: ���� ������ ��/�� ��ȣ Ȯ���ؼ� �ѱ�
        var hoof = hitbox.GetComponent<AB_HoofOut>();
        if (hoof != null)
        {
            // 1) �⺻: ���� ������ x �������� ��/�� ����
            float sign = Mathf.Abs(direction.x) > 0.0001f ? Mathf.Sign(direction.x) : 0f;

            // 2) ���� x�� ���� 0�̸�(���� ���� ��), ���� ��ġ�� owner�� ��� �������� ����
            if (sign == 0f && playerAbility != null)
                sign = Mathf.Sign(spawnPos.x - playerAbility.transform.position.x);

            // 3) �׷��� 0�̸� �⺻ ����(+1)
            if (sign == 0f) sign = 1f;

            hoof.SetLateralSign(sign);
        }
    }
}