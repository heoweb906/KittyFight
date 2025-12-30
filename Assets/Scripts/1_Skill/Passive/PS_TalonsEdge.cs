using UnityEngine;

public class PS_TalonsEdge : Passive
{
    public override int PassiveId => 108;

    [Header("Talons' Edge")]
    public int basePoints = 5;    // 라운드 시작마다 이 값으로
    public int addPerUse = 2;     // 근접 "사용"할 때마다 +0.2

    private int currentPoints;
    private int swingCount;

    [Header("카메라 연출")]
    public float baseShake = 0.10f;
    public float shakeStep = 0.07f;     // 반복할수록 더 강하게
    public float maxShake = 0.45f;      // 상한
    public float shakeDuration = 0.12f;

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
    swingCount = 0;
    }


    void HandleMeleeDamageInt(ref int damage)
    {
        damage = currentPoints;
        currentPoints += addPerUse;

        swingCount++;

        float strength = Mathf.Clamp(baseShake + swingCount * shakeStep, baseShake, maxShake);

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(strength, shakeDuration);
    }
}