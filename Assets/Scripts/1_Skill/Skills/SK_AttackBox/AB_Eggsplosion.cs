using UnityEngine;

public class AB_Eggsplosion : MonoBehaviour
{
    public int ownerPlayerNumber;
    public float blindnessDuration = 2.5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null || health.playerNumber == ownerPlayerNumber) return;
        //if (health.playerNumber != MatchResultStore.myPlayerNumber) return;

        // 시야 방해 부착
        var blindness = other.gameObject.AddComponent<BlindStatus>();
        blindness.ApplyBlind(blindnessDuration);

        //Destroy(gameObject);
    }
}