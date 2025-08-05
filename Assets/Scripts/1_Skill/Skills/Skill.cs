using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISKILL
{
    void Activate();
}

public abstract class Skill : MonoBehaviour, ISKILL 
{
    [Header("�⺻������ ����ϴ� �͵�")]
    public PlayerAbility playerAbilty;
    public SkillWorker skillWorker;


    [Header("�ϳ��� ��ų�̱� ���ؼ� ���������� ������ �ִ� �͵�")]
    public float coolTime;
    public GameObject objSkillEntity;

    private float lastUseTime = -Mathf.Infinity;



    public Skill(PlayerAbility playerAbilty, SkillWorker skillWorker)
    {
        this.playerAbilty = playerAbilty;
        this.skillWorker = skillWorker;
    }

    // ������ ������� ���� �� ȣ���ϴ� �ʱ�ȭ �Լ�
    public void SetNewBasicValue(PlayerAbility playerAbilty, SkillWorker skillWorker)
    {
        this.playerAbilty = playerAbilty;
        this.skillWorker = skillWorker;
    }




    public void Activate()
    {
        if (Time.time < lastUseTime + coolTime)
        {
            Debug.Log($"Cool Time: {Mathf.Ceil(lastUseTime + coolTime - Time.time)}");
            return;
        }

        lastUseTime = Time.time;
        ExecuteSkill();
    }

    protected abstract void ExecuteSkill();
}
