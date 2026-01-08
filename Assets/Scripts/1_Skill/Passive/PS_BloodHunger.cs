using UnityEngine;

public class PS_BloodHunger : Passive
{
    public override int PassiveId => 107;

    [Header("블러드 헝거 설정")]
    public int hitsPerHeal = 3;   // 적에게 3번 피해를 입힐 때마다
    public int healAmount = 2;    // 체력 2 회복

    private int hitCount = 0;
    private PlayerHealth ownerHealth;

    [Header("이펙트")]
    public GameObject objEffect_Use;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);

        if (ability != null)
            ownerHealth = ability.GetComponent<PlayerHealth>();

        e.OnDealtDamage += OnDealtDamage;
        e.OnRoundStart += OnRoundStart;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnDealtDamage -= OnDealtDamage;
        e.OnRoundStart -= OnRoundStart;
        base.Unsubscribe(e);
    }

    private void OnDealtDamage()
    {
        if (!IsAuthority) return;
        if (ownerHealth == null) return;
        if (hitsPerHeal <= 0) return;

        hitCount++;

        if (hitCount < hitsPerHeal) return;
        hitCount -= hitsPerHeal;

        ownerHealth.Heal(healAmount);

        PlayFx(transform.position);
        SendProc(
            PassiveProcType.FxOnly,
            pos: transform.position,
            dir: Vector3.up,
            i0: healAmount,
            f0: 0f
        );

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    private void PlayFx(Vector3 pos)
    {
        if (objEffect_Use == null) return;

        GameObject effect = Instantiate(objEffect_Use, pos, Quaternion.Euler(-90, 0, 0));
        effect.transform.localScale = Vector3.one;

        ability.PlaySFX(audioClip);
    }
    public override void RemoteExecute(PassiveProcMessage msg)
    {
        var pos = new Vector3(msg.px, msg.py, msg.pz);
        PlayFx(pos);
    }

    private void OnRoundStart(int roundIndex)
    {
        if (!IsAuthority) return;
        hitCount = 0;
    }
}