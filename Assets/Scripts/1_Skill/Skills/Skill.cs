using UnityEngine;

public interface ISKILL
{
    void Execute(Vector3 origin, Vector3 direction);
}

public abstract class Skill : MonoBehaviour, ISKILL
{
    [Header("기본 연결")]
    public PlayerAbility playerAbility;

    [Header("스킬 설정(데이터)")]
    public float coolTime;   // Ability가 참조하는 데이터
    public GameObject objSkillEntity;
    public Sprite skillIcon;

    protected SkillType assignedSlot;

    public void SetNewBasicValue(PlayerAbility ability) => playerAbility = ability;
    public void SetAssignedSlot(SkillType slot) => assignedSlot = slot;

    // 래퍼 없이 순수 추상, 각 스킬은 이걸 구현
    public abstract void Execute(Vector3 origin, Vector3 direction);
}