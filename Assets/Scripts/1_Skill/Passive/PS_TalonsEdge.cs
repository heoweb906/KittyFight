using UnityEngine;

public class PS_TalonsEdge : Passive
{
    public override int PassiveId => 108;

    [Header("Talons' Edge")]
    public int basePoints = 5;    // 라운드 시작마다 이 값으로
    public int addPerUse = 2;     // 근접 "사용"할 때마다 +0.2

    private int currentPoints;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnRoundStart += HandleRoundStart;
        e.OnMeleeDamageInt += HandleMeleeDamageInt;

        // 장착 직후 초기화
        HandleRoundStart(0);
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnRoundStart -= HandleRoundStart;
        e.OnMeleeDamageInt -= HandleMeleeDamageInt;
    }

    void HandleRoundStart(int _)
    {
        currentPoints = basePoints;
        shakeAmount = 0.1f;
    }


    void HandleMeleeDamageInt(ref int damage)
    {
        damage = currentPoints;

        currentPoints += addPerUse;

        var gm = FindObjectOfType<GameManager>();

        shakeAmount += 0.1f;

        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }
}