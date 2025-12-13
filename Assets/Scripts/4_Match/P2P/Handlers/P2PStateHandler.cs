using UnityEngine;

public class P2PStateHandler : IP2PMessageHandler
{
    private GameObject opponentPlayer;
    private Animator anim;
    private int myPlayerNumber;
    private GameManager gameManager;

    public P2PStateHandler(GameObject opponentObj, int myNumber, GameManager gm)
    {
        opponentPlayer = opponentObj;
        anim = opponentObj.GetComponentInChildren<Animator>();
        myPlayerNumber = myNumber;
        gameManager = gm;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[MOVE]");

    public void Handle(string msg)
    {
        gameManager?.NotifyOpponentStateReceived();

        var state = JsonUtility.FromJson<PlayerState>(msg.Substring(6));
        if (state.player == myPlayerNumber) return;
        if (opponentPlayer == null) return;

        opponentPlayer.transform.position = state.position;
        opponentPlayer.transform.rotation = Quaternion.Euler(0, state.rotationY, 0);

        var dir = Quaternion.Euler(0f, state.rotationY, 0f) * Vector3.forward;
        bool facingRight = dir.x > 0f;

        var pm = opponentPlayer.GetComponent<PlayerMovement>();
        if (pm != null && pm.visualPivot != null)
        {
            float yaw = facingRight ? 50f : 310f;
            pm.visualPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
        }

        // ¿Ã∆Â∆Æ √≥∏Æ
        var pj = opponentPlayer.GetComponent<PlayerJump>();
        if (pj != null)
        {
            pj.SetWalking(state.walking);

            // Jump ø¯º¶ ¿Ã∆Â∆Æ
            if (!string.IsNullOrEmpty(state.anim) && state.anim == "Jump" && pj.jumpEffectPrefab != null)
            {
                Object.Instantiate(
                    pj.jumpEffectPrefab,
                    opponentPlayer.transform.position,
                    Quaternion.Euler(-90f, 0f, 0f)
                );
            }
        }

        if (anim != null)
        {
            anim.SetBool("isGround", state.isGround);
            anim.SetBool("isRun", state.isRun);
            anim.SetBool("isHanging", state.isHanging);
            anim.SetFloat("speedY", state.speedY);
            anim.SetBool("isShock", state.isShock);
        }
    }
}