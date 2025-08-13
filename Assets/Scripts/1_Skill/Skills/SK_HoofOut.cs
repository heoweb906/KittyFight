// SK_HoofOut.cs
using UnityEngine;

public class SK_HoofOut : Skill
{
    public float maxRange = 3.0f;
    public float hitboxLifeTime = 1.0f;

    private void Awake()
    {
        // 필요시 쿨타임 설정 (Ability가 조회만 함)
        coolTime = 3.0f; 
    }

    // 좌표/방향은 호출자가 계산해서 넘겨줌
    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Vector3 spawnPos = origin + direction * maxRange;
        Quaternion rot   = Quaternion.LookRotation(direction);

        if (objSkillEntity != null)
        {
            GameObject hitbox = Instantiate(objSkillEntity, spawnPos, rot);

            // 공통 베이스에 소유자 주입 (playerNumber 포함)
            var ab = hitbox.GetComponent<AB_HitboxBase>();
            if (ab != null)
            {
                // 수명 커스터마이즈가 필요하면 캐스트해서 lifeTime도 세팅 가능
                ab.Init(playerAbility);
            }

            if (hitboxLifeTime > 0f) Destroy(hitbox, hitboxLifeTime);
        }
    }
}