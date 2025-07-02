using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISKILL
{
    void Activate();
}

public abstract class Skill : ISKILL
{
    [Header("기본적으로 사용하는 것들")]
    public PlayerAbility playerAbilty;
    public SkillWorker skillWorker;


    [Header("하나의 스킬이기 위해서 공통적으로 가지고 있는 것들")]
    public float coolTime;
    public GameObject objSkillEntity;

    private float lastUseTime = -Mathf.Infinity; // 최소 값으로 초기화




    public Skill(PlayerAbility playerAbilty, SkillWorker skillWorker)
    {
        this.playerAbilty = playerAbilty;
        this.skillWorker = skillWorker;
    }

    // 프리팹 기반으로 생성 시 호출하는 초기화 함수
    public void SetNewBasicValue(PlayerAbility playerAbilty, SkillWorker skillWorker)
    {
        this.playerAbilty = playerAbilty;
        this.skillWorker = skillWorker;
    }


    public void Activate()
    {
        if (Time.time < lastUseTime + coolTime)
        {
            Debug.Log($"남은 스킬 쿨타임: {Mathf.Ceil(lastUseTime + coolTime - Time.time)}");
            return;
        }

        lastUseTime = Time.time;
        ExecuteSkill();
    }

    protected abstract void ExecuteSkill();
}
