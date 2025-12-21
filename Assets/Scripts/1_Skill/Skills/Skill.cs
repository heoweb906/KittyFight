using UnityEngine;

public interface ISKILL
{
    void Execute(Vector3 origin, Vector3 direction);
}

public abstract class Skill : MonoBehaviour, ISKILL
{
    public int SkillIndex { get; set; }

    [Header("기본 연결")]
    public PlayerAbility playerAbility;
    public Animator anim;

    [Header("Audio")]
    public AudioClip sfxClip;

    [Header("스킬 설정(데이터)")]
    public float coolTime;   // Ability가 참조하는 데이터 
    public GameObject objSkillEntity;
    public Sprite skillIcon;

    [Header("조준/사거리")]
    public float aimRange = 2.5f;

    protected SkillType assignedSlot;

    public void SetNewBasicValue(PlayerAbility ability) => playerAbility = ability;
    public void SetAssignedSlot(SkillType slot) => assignedSlot = slot;
    public virtual float GetAimRange() => aimRange;
    public virtual void Bind(PlayerAbility ability) {
        playerAbility = ability;
        anim = ability.GetComponentInChildren<Animator>();
    }

    // 래퍼 없이 순수 추상, 각 스킬은 이걸 구현
    public abstract void Execute(Vector3 origin, Vector3 direction);
}