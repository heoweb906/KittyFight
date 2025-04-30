using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Null : Skill
{
    public override void Activate(PlayerAbility _ability)
    {
        Debug.Log("기본 스킬이 들어가 있습니다");
        // 여기에 방어막 생성 로직
    }
}
