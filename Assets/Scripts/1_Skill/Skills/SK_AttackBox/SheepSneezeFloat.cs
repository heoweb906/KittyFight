using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepSneezeFloat : MonoBehaviour
{
    [Header("초기 움직임 설정")]
    [Tooltip("초기 이동 속도 (털뭉치가 얼마나 멀리 나갈지)")]
    public float initialSpeed = 3f;

    [Tooltip("중력을 유지할 시간 (초)")]
    public float gravityDuration = 0.5f;

    private Rigidbody rb;
    private bool started = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        Vector3 dir = direction.normalized;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = dir * initialSpeed;
        rb.angularVelocity = Vector3.zero;

        if (!started)
        {
            started = true;
            StartCoroutine(GravityThenFloatRoutine());
        }
    }

    private IEnumerator GravityThenFloatRoutine()
    {
        yield return new WaitForSeconds(gravityDuration);

        if (rb == null) yield break;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
    }
}