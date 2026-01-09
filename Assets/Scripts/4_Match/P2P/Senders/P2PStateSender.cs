using UnityEngine;

public class P2PStateSender : P2PManager
{
    public static GameObject myPlayer;
    private static Animator anim;
    private static int myPlayerNumber;
    private static PlayerHealth myHealth;

    public static void Init(GameObject playerObj, int playerNumber)
    {
        myPlayer = playerObj;
        anim = playerObj != null ? playerObj.GetComponentInChildren<Animator>() : null;
        myPlayerNumber = playerNumber;
    }

    public static void SendMyState()
    {
        if (myPlayer == null) return;

        string evtOnce = UpdateManager.ConsumeEventOnce();
        
        bool walking = false;
        var pj = myPlayer.GetComponent<PlayerJump>();
        if (pj != null) walking = pj.IsWalking;

        bool isGround = false, isRun = false, isHanging = false, isShock = false, isHangRight = false;
        float speedY = 0f;

        if (anim != null)
        {
            // 파라미터 이름은 양쪽 Animator에서 동일해야 함
            isGround = anim.GetBool("isGround");
            isRun = anim.GetBool("isRun");
            isHanging = anim.GetBool("isHanging");
            speedY = anim.GetFloat("speedY");
            isShock = anim.GetBool("isShock");
            isHangRight = anim.GetBool("isHangRight");
        }

        if (myHealth == null) myHealth = myPlayer.GetComponent<PlayerHealth>();

        var msg = PlayerStateMessageBuilder.Build(
            myPlayer.transform.position,
            myPlayer.transform.eulerAngles.y,
            myPlayerNumber,
            evtOnce,
            walking,
            isGround,
            isRun,
            isHanging,
            speedY,
            isShock,
            isHangRight
        );

        P2PMessageSender.SendMessage(msg);
    }
}