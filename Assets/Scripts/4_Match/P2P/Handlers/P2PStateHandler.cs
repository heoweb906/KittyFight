using System;
using UnityEngine;

public class P2PStateHandler : IP2PMessageHandler
{
    private const string Prefix = "[MOVE]";

    private GameObject opponentPlayer;
    private Animator anim;
    private int myPlayerNumber;
    private GameManager gameManager;

    public P2PStateHandler(GameObject opponentObj, int myNumber, GameManager gm)
    {
        opponentPlayer = opponentObj;
        anim = opponentObj != null ? opponentObj.GetComponentInChildren<Animator>() : null;
        myPlayerNumber = myNumber;
        gameManager = gm;
    }

    public bool CanHandle(string msg) => !string.IsNullOrEmpty(msg) && msg.StartsWith(Prefix);

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;
        if (string.IsNullOrEmpty(msg)) return;

        if (!msg.StartsWith(Prefix)) return;
        if (msg.Length <= Prefix.Length) return;

        gameManager?.NotifyOpponentStateReceived();

        string json = msg.Substring(Prefix.Length);
        if (string.IsNullOrWhiteSpace(json)) return;

        PlayerState state;
        try
        {
            state = JsonUtility.FromJson<PlayerState>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][MOVE] JSON parse failed. msgLen={msg.Length} err={e.Message}");
            return;
        }

        if (state == null) return;
        if (state.player == myPlayerNumber) return;

        if (opponentPlayer == null) return;

        var tr = opponentPlayer.transform;
        if (tr == null) return;

        tr.position = state.position;
        tr.rotation = Quaternion.Euler(0, state.rotationY, 0);

        var dir = Quaternion.Euler(0f, state.rotationY, 0f) * Vector3.forward;
        bool facingRight = dir.x > 0f;

        var pm = opponentPlayer.GetComponent<PlayerMovement>();
        if (pm != null && pm.visualPivot != null)
        {
            float yaw = facingRight ? 50f : 310f;
            pm.visualPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
        }

        var pj = opponentPlayer.GetComponent<PlayerJump>();
        if (pj != null)
        {
            pj.SetWalking(state.walking);

            if (!string.IsNullOrEmpty(state.anim) && state.anim == "Jump" && pj.jumpEffectPrefab != null)
            {
                try
                {
                    UnityEngine.Object.Instantiate(
                        pj.jumpEffectPrefab,
                        tr.position,
                        Quaternion.Euler(-90f, 0f, 0f)
                    );
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[P2P][MOVE] Instantiate jumpEffect failed: {e.Message}");
                }

                pj.PlayJumpSfx();
            }
        }

        if (anim != null)
        {
            anim.SetBool("isGround", state.isGround);
            anim.SetBool("isRun", state.isRun);
            anim.SetBool("isHanging", state.isHanging);
            anim.SetFloat("speedY", state.speedY);
            anim.SetBool("isShock", state.isShock);
            anim.SetBool("isHangRight", state.isHangRight);
        }
    }
}