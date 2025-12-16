using UnityEngine;

public class PS_FeralBloodlust : Passive
{
    public override int PassiveId => 134;

    [Header("피해량 증폭 설정")]
    [Tooltip("적에게 주는 피해에 더해질 값")]
    public int extraDealDamage = 20;

    [Tooltip("내가 받는 피해에 더해질 값")]
    public int extraTakenDamage = 20;

    [Header("이펙트")]
    public GameObject objEffect_Use;

    private const int PROC_DEAL_FX = 1;
    private const int PROC_TAKE_FX = 2;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnBeforeDealDamage += OnBeforeDealDamage;
        e.OnBeforeTakeDamage += OnBeforeTakeDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnBeforeDealDamage -= OnBeforeDealDamage;
        e.OnBeforeTakeDamage -= OnBeforeTakeDamage;
    }

    private void OnBeforeDealDamage(ref int dmg, GameObject victim)
    {
        if (dmg <= 0) return;
        if (extraDealDamage != 0)
            dmg += extraDealDamage;

        if (victim == null || objEffect_Use == null) return;

        Vector3 fxPos = victim.transform.position;

        PlayFxAtWorld(fxPos);
        SendProcOverrideSenderNoAuthority(
            senderPlayerNumber: MatchResultStore.myPlayerNumber,
            procType: PassiveProcType.FxOnly,
            pos: fxPos,
            dir: Vector3.up,
            i0: PROC_DEAL_FX
        );
    }

    private void OnBeforeTakeDamage(ref int dmg, GameObject attacker)
    {
        if (dmg <= 0) return;
        if (extraTakenDamage != 0)
            dmg += extraTakenDamage;

        if (objEffect_Use == null) return;

        Vector3 fxPos = transform.position;

        PlayFxAtWorld(fxPos);
        SendProc(
            PassiveProcType.FxOnly,
            pos: fxPos,
            dir: Vector3.up,
            i0: PROC_TAKE_FX
        );
    }

    private void PlayFxAtWorld(Vector3 pos)
    {
        Instantiate(
            objEffect_Use,
            pos,
            Quaternion.Euler(-90f, 0f, 0f)
        );
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        Vector3 pos = new Vector3(msg.px, msg.py, msg.pz);
        PlayFxAtWorld(pos);
    }
}