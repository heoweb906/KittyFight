using UnityEngine;

public class PS_WoollyGuard : Passive
{
    public override int PassiveId => 120;

    [Header("받는 피해 감소 설정")]
    [Tooltip("받는 피해 감소")]
    public int damageDivisor = 12;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnBeforeTakeDamage += OnBeforeTakeDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnBeforeTakeDamage -= OnBeforeTakeDamage;
    }

    private void OnBeforeTakeDamage(ref int dmg, GameObject attacker)
    {
        if (!IsAuthority) return;
        if (dmg <= 0) return;
        if (damageDivisor <= 0) return;

        dmg -= damageDivisor;
        if (dmg < 0) dmg = 0;

        PlayFx(transform.position);
        SendProc(
            PassiveProcType.FxOnly,
            pos: transform.position,
            dir: Vector3.up
        );

        ability.PlaySFX(audioClip);
    }

    private void PlayFx(Vector3 pos)
    {
        if (!effectPrefab) return;

        Instantiate(
            effectPrefab,
            pos,
            Quaternion.identity,
            transform
        );
    }
    public override void RemoteExecute(PassiveProcMessage msg)
    {
        var pos = new Vector3(msg.px, msg.py, msg.pz);
        PlayFx(pos);
    }
}