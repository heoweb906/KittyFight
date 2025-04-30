using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Shield : Skill
{
    public override void Activate(PlayerAbility _ability)
    {
        Debug.Log("Shield Skill Activated!"); 
        // 여기에 방어막 생성 로직
    }
}
