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

        // ����Ʈ ó��
        var pj = opponentPlayer.GetComponent<PlayerJump>();
        if (pj != null)
        {
            pj.SetWalking(state.walking);

            // Jump ���� ����Ʈ
            if (!string.IsNullOrEmpty(state.anim) && state.anim == "Jump" && pj.jumpEffectPrefab != null)
            {
                Object.Instantiate(
                    pj.jumpEffectPrefab,
                    opponentPlayer.transform.position,
                    Quaternion.Euler(-90f, 0f, 0f)
                );
            }
        }
    }
}