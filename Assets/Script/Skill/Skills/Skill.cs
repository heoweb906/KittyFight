using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SKILL이라면 반드시 구현되야 할 함수들
public interface ISKILL
{
    void Activate();
}

// SKILL에 포함될 다양한 기능들
public abstract class Skill : ISKILL
{
    public PlayerAbility playerAbilty;
    public PlayerSkillAbilty skillAbilty;

    public float coolTime;
    public GameObject objSkillEntity;


    private float lastUseTime = -Mathf.Infinity; // 최소 값으로 초기화 하는 코드임 

    // 생성자 명시
    public Skill(PlayerAbility playerAbilty, PlayerSkillAbilty skillAbilty)
    {
        this.playerAbilty = playerAbilty;
        this.skillAbilty = skillAbilty;
    }

    public void Activate()
    {
        if (Time.time < lastUseTime + coolTime)
        {
            Debug.Log($" 남은 스킬 쿨타임: {Mathf.Ceil(lastUseTime + coolTime - Time.time)}");
            return;
        }

        lastUseTime = Time.time;
        ExecuteSkill(); 
    }

    protected abstract void ExecuteSkill();
}