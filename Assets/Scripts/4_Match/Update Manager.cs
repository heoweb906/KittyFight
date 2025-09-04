using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private bool isReady = false;

    private static string s_evtOnce;

    public static void EnqueueEventOnce(string evt)
    {
        s_evtOnce = evt; // ������ �̺�Ʈ�� ����
    }

    public static string ConsumeEventOnce()
    {
        var t = s_evtOnce;
        s_evtOnce = null;
        return t;
    }

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
        //Debug.Log("[UpdateManager] FixedUpdate ���� ��");
        P2PStateSender.SendMyState();
    }
}