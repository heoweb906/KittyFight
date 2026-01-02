using System.Collections;
using UnityEngine;

public class StunStatus : MonoBehaviour
{
    private MonoBehaviour controller;
    private PlayerAbility ability;
    private Coroutine stunRoutine;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        ability = GetComponent<PlayerAbility>();
    }

    public void ApplyStun(float duration)
    {
        ApplyStun(duration, true);
    }

    public void ApplyStun(float duration, bool playShockAnim)
    {
        controller = GetComponent<PlayerController>(); // 조작 차단 대상
        if (controller != null)
        {
            controller.enabled = false;
            if (stunRoutine != null) StopCoroutine(stunRoutine);
            stunRoutine = StartCoroutine(StunTimer(duration));
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        if (anim != null && !playShockAnim)
        {
            anim.SetBool("isRun", false);
        }
        else if (anim != null && playShockAnim)
            anim.SetBool("isShock", true);
    }

    private IEnumerator StunTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (controller != null && ability != null &&
            ability.playerNumber == MatchResultStore.myPlayerNumber)
        {
            controller.enabled = true;
        }

        anim.SetBool("isShock", false);
        Destroy(this);
    }
}