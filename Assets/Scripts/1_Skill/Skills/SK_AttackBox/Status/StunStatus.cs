using System.Collections;
using UnityEngine;

public class StunStatus : MonoBehaviour
{
    private MonoBehaviour controller;
    private Coroutine stunRoutine;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void ApplyStun(float duration)
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
        anim.SetBool("isShock", true);
    }

    private IEnumerator StunTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (controller != null)
        {
            controller.enabled = true;
        }
        anim.SetBool("isShock", false);
        Destroy(this);
    }
}