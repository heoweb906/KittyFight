using UnityEngine;

public class SK_MysteryFruit : Skill
{
    [Header("HP 변화 범위(정수)")]
    [SerializeField] private int minDelta = -20;
    [SerializeField] private int maxDelta = 30;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    public float shakeAmount;
    public float shakeDeration;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.identity,
            playerAbility.gameObject.transform
        );

        int pn = playerAbility != null ? playerAbility.playerNumber : 0;
        if (pn != MatchResultStore.myPlayerNumber) return;

        var ph = (playerAbility ? playerAbility.GetComponent<PlayerHealth>() : null)
                 ?? GetComponent<PlayerHealth>();
        if (!ph) return;
        int delta = Random.Range(minDelta, maxDelta + 1);

        int cur = ph.CurrentHP;
        int max = ph.MaxHP;
        int newHP = Mathf.Clamp(cur + delta, 0, max);

        Debug.Log(newHP);
        ph.RemoteSetHP(newHP);

        P2PMessageSender.SendMessage(
            DamageMessageBuilder.Build(pn, newHP, 0, null)
        );

        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeAmount, direction);
        }
    }
}