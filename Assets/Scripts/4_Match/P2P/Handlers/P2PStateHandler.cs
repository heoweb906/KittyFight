using UnityEngine;

public class P2PStateHandler : IP2PMessageHandler
{
    private GameObject opponentPlayer;
    private Animator anim;
    private int myPlayerNumber;

    public P2PStateHandler(GameObject opponentObj, int myNumber)
    {
        opponentPlayer = opponentObj;
        anim = opponentObj.GetComponent<Animator>();
        myPlayerNumber = myNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[MOVE]");

    public void Handle(string msg)
    {
        var state = JsonUtility.FromJson<PlayerState>(msg.Substring(6));
        if (state.player == myPlayerNumber) return;

        opponentPlayer.transform.position = state.position;
        opponentPlayer.transform.rotation = Quaternion.Euler(0, state.rotationY, 0);

        if (anim != null && !string.IsNullOrEmpty(state.anim))
            anim.SetTrigger(state.anim);
    }
}