using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private bool isReady = false;

    public void Initialize(GameObject myPlayer, GameObject opponentPlayer, int playerNumber)
    {
        P2PStateSender.Init(myPlayer, playerNumber);
        //P2PStateReceiver.Init(opponentPlayer, playerNumber);

        isReady = true;
        //Debug.Log($"[UpdateManager] Initialized. I am Player {playerNumber}");
    }

    void FixedUpdate()
    {
        if (!isReady) return;
        //Debug.Log("[UpdateManager] FixedUpdate ½ÇÇà Áß");
        P2PStateSender.SendMyState();
    }
}