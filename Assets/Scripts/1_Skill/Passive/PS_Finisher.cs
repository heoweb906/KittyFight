using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Finisher : Passive
{
    public override int PassiveId => 133;

    [Range(0f, 1f)] public float threshold = 0.20f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;


    protected override void Subscribe(AbilityEvents e)
        => e.OnBeforeDealDamage += OnBeforeDealDamage;
    protected override void Unsubscribe(AbilityEvents e)
        => e.OnBeforeDealDamage -= OnBeforeDealDamage;

    // 얘만은 권위자 세팅을 하면안됨..
    void OnBeforeDealDamage(ref int dmg, GameObject victimGO)
    {
        if (victimGO == null) return;

        var hp = victimGO.GetComponentInParent<PlayerHealth>();
        if (!hp) return;

        int cur = hp.CurrentHP;
        int max = hp.MaxHP;

        if (max <= 0 || cur <= 0) return;

        float ratio = (float)cur / max;
        if (ratio > threshold) return;

        dmg = 9999;
        Vector3 fxPos = victimGO.transform.position;

        PlayFx(fxPos);
        SendProcOverrideSenderNoAuthority(
            senderPlayerNumber: MatchResultStore.myPlayerNumber,
            procType: PassiveProcType.FxOnly,
            pos: fxPos,
            dir: Vector3.up
        );

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    private void PlayFx(Vector3 pos)
    {
        if (!effectPrefab) return;

        Instantiate(
            effectPrefab,
            pos,
            Quaternion.Euler(-90f, 0f, 0f)
        );
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        var pos = new Vector3(msg.px, msg.py, msg.pz);
        PlayFx(pos);
    }
}