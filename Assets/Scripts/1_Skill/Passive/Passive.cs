using UnityEngine;

public abstract class Passive : MonoBehaviour
{
    protected PlayerAbility ability;
    protected AbilityEvents events;

    public Sprite skillIcon;

    public abstract int PassiveId { get; } // 고유 id
    protected bool IsAuthority =>
        ability != null && ability.playerNumber == MatchResultStore.myPlayerNumber;

    public virtual void OnEquip(PlayerAbility a)
    {
        ability = a;
        events = a.events;
        events = a.GetComponent<AbilityEvents>();
        if (events == null) events = a.gameObject.AddComponent<AbilityEvents>();
        Subscribe(events);
    }


    public virtual void OnUnequip()
    {
        if (events != null) Unsubscribe(events);
        events = null; ability = null;
    }


    protected virtual void Subscribe(AbilityEvents e) { }
    protected virtual void Unsubscribe(AbilityEvents e) { }

    // 원격 실행용
    public virtual void RemoteExecute(PassiveProcMessage msg) { }

    protected void SendProc(
        PassiveProcType procType,
        Vector3 pos,
        Vector3 dir,
        int i0 = 0,
        float f0 = 0f,
        int targetPlayer = 0
    )
    {
        if (!IsAuthority) return;
        if (ability == null) return;

        P2PMessageSender.SendMessage(
            PassiveMessageBuilder.Build(
                casterPlayerNumber: ability.playerNumber,
                passiveId: PassiveId,
                procType: procType,
                pos: pos,
                dir: dir,
                i0: i0,
                f0: f0,
                targetPlayer: targetPlayer
            )
        );
    }

    protected void SendProcOverrideSenderNoAuthority(
        int senderPlayerNumber,
        PassiveProcType procType,
        Vector3 pos,
        Vector3 dir,
        int i0 = 0,
        float f0 = 0f,
        int targetPlayer = 0
    )
    {
        if (ability == null) return;

        P2PMessageSender.SendMessage(
            PassiveMessageBuilder.Build(
                casterPlayerNumber: senderPlayerNumber,
                passiveId: PassiveId,
                procType: procType,
                pos: pos,
                dir: dir,
                i0: i0,
                f0: f0,
                targetPlayer: targetPlayer
            )
        );
    }
}