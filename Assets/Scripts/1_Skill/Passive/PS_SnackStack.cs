using UnityEngine;

public class PS_SnackStack : Passive
{
    public override int PassiveId => 137;

    [Header("Snack & Stack")]
    public int initialMaxHP = 50;   // 장착 즉시 5
    public int growPerRound = 20;    // 라운드 시작마다 +2
    public bool keepRatioOnChange = false; // 보통 false: 힐은 안 됨


    [Header("이펙트")]
    public GameObject objEffect_Use;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;


    protected override void Subscribe(AbilityEvents e)
    {
        e.OnRoundStart += OnRoundStart;

        var hp = ability.GetComponent<PlayerHealth>();
        if (!hp) return;

        hp.SetMaxHP(initialMaxHP, keepRatioOnChange);
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnRoundStart -= OnRoundStart;
    }

    void OnRoundStart(int roundIndex)
    {
        var hp = ability.GetComponent<PlayerHealth>();
        if (!hp) return;

        hp.AddMaxHP(growPerRound, keepRatioOnChange);

        if (objEffect_Use != null)
        {
            GameObject effect = Instantiate(objEffect_Use, transform);
            effect.transform.localPosition = Vector3.zero;

            effect.transform.rotation = Quaternion.Euler(-90, 0, 0);

            effect.transform.localScale = new Vector3(1f, 1f, 1f);
            effect.transform.SetParent(null);
        }

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);

    }
}