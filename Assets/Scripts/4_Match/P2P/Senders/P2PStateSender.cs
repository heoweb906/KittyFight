using UnityEngine;

public class P2PStateSender : P2PManager
{
    public static GameObject myPlayer;
    private static Animator anim;
    private static int myPlayerNumber;

    public static void Init(GameObject playerObj, int playerNumber)
    {
        myPlayer = playerObj;
        anim = playerObj.GetComponent<Animator>();
        myPlayerNumber = playerNumber;
    }

    public static void SendMyState()
    {
        if (myPlayer == null) return;

        string evtOnce = UpdateManager.ConsumeEventOnce(); // "Jump" ¶Ç´Â null
        //string animState = GetCurrentAnim();
        
        bool walking = false;
        var pj = myPlayer.GetComponent<PlayerJump>();
        if (pj != null) walking = pj.IsWalking;

        var msg = PlayerStateMessageBuilder.Build(
            myPlayer.transform.position,
            myPlayer.transform.eulerAngles.y,
            myPlayerNumber,
            evtOnce,
            walking
        );

        P2PMessageSender.SendMessage(msg);
    }

    static string GetCurrentAnim()
    {
        if (anim == null) return "idle";

        var info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Idle")) return "idle";
        if (info.IsName("Run")) return "run";
        if (info.IsName("Jump")) return "jump";
        return "unknown";
    }
}