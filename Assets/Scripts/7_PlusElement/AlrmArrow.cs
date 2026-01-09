using UnityEngine;

public class AlrmArrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float autoHideDelay = 10f; // 대기 시간

    private Transform targetTransform;
    private Vector3 offset;

    private void OnEnable()
    {
        // 10초 뒤 오브젝트 파괴
        Destroy(gameObject, autoHideDelay);
    }

    private void LateUpdate()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position + offset;
        }
    }

    public void SetTarget(Transform target, Vector3 offsetVector)
    {
        targetTransform = target;
        offset = offsetVector;
    }
}