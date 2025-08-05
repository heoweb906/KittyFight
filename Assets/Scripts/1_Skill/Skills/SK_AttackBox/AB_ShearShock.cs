using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_ShearShock : MonoBehaviour
{
    public float stunDuration = 2f;
    public int ownerPlayerNumber;

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null || health.playerNumber == ownerPlayerNumber) return;
        //if (health.playerNumber != MatchResultStore.myPlayerNumber) return;

        // ���� ���� �ο�
        var stun = other.gameObject.AddComponent<StunStatus>();
        stun.ApplyStun(stunDuration);
    }
}
