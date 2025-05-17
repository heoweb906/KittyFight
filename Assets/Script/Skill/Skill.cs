using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SKILL�̶�� �ݵ�� �����Ǿ� �� �Լ���
public interface ISKILL
{
    void Activate();
}

// SKILL�� ���Ե� �پ��� ��ɵ�
public abstract class Skill : ISKILL
{
    public PlayerAbility playerAbilty;
    public PlayerSkillAbilty skillAbilty;

    public float coolTime;
    public GameObject objSkillEntity;


    private float lastUseTime = -Mathf.Infinity; // �ּ� ������ �ʱ�ȭ �ϴ� �ڵ��� 

    // ������ ���
    public Skill(PlayerAbility playerAbilty, PlayerSkillAbilty skillAbilty)
    {
        this.playerAbilty = playerAbilty;
        this.skillAbilty = skillAbilty;
    }

    public void Activate()
    {
        if (Time.time < lastUseTime + coolTime)
        {
            Debug.Log($" ���� ��ų ��Ÿ��: {Mathf.Ceil(lastUseTime + coolTime - Time.time)}");
            return;
        }

        lastUseTime = Time.time;
        ExecuteSkill(); 
    }

    protected abstract void ExecuteSkill();
}