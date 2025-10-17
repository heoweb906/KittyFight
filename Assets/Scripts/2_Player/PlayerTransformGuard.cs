using UnityEngine;

public class PlayerTransformGuard : MonoBehaviour
{
    private Transform initialParent;
    private Vector3 initialLocalPosition;

    private const float Tolerance = 0.0001f;

    void Awake()
    {
        initialParent = transform.parent;
        initialLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (transform.parent != initialParent)
        {
            transform.SetParent(initialParent);
        }

        if (Vector3.Distance(transform.localPosition, initialLocalPosition) > Tolerance)
        {
            transform.localPosition = initialLocalPosition;
        }
    }
}