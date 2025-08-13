// SK_HoofOut.cs
using UnityEngine;

public class SK_HoofOut : Skill
{
    public float maxRange = 3.0f;
    public float hitboxLifeTime = 1.0f;

    private void Awake()
    {
        // �ʿ�� ��Ÿ�� ���� (Ability�� ��ȸ�� ��)
        coolTime = 3.0f; 
    }

    // ��ǥ/������ ȣ���ڰ� ����ؼ� �Ѱ���
    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Vector3 spawnPos = origin + direction * maxRange;
        Quaternion rot   = Quaternion.LookRotation(direction);

        if (objSkillEntity != null)
        {
            GameObject hitbox = Instantiate(objSkillEntity, spawnPos, rot);

            // ���� ���̽��� ������ ���� (playerNumber ����)
            var ab = hitbox.GetComponent<AB_HitboxBase>();
            if (ab != null)
            {
                // ���� Ŀ���͸���� �ʿ��ϸ� ĳ��Ʈ�ؼ� lifeTime�� ���� ����
                ab.Init(playerAbility);
            }

            if (hitboxLifeTime > 0f) Destroy(hitbox, hitboxLifeTime);
        }
    }
}