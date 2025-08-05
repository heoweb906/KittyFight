using UnityEngine;

public class AB_FangShot : MonoBehaviour
{
    public int ownerPlayerNumber;
    public float duration = 3.0f;
    public float tickInterval = 1.0f;
    public int damagePerTick = 2;

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        //if (hasHit) return;

        var health = other.GetComponent<PlayerHealth>();
        if (health == null || health.playerNumber == ownerPlayerNumber) return;
        //if (health.playerNumber != MatchResultStore.myPlayerNumber) return;

        hasHit = true;

        // 도트 데미지 부착
        var poison = other.gameObject.AddComponent<PoisonDoT>();
        poison.ApplyPoison(duration, tickInterval, damagePerTick);

        //Destroy(gameObject);
    }
}
