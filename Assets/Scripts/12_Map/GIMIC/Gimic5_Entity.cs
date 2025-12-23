using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimic5_Entity : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetObject; // 레이저 프리펩

    public GameObject obj_1;
    public GameObject obj_2;

    void Start()
    {
        // 생성 시점의 회전값에 따라 스케일 조정
        AdjustScaleByRotation();
        StartCoroutine(Co_SpawnProcess());
    }

    // 회전 각도에 맞춰 X축 크기 변경
    private void AdjustScaleByRotation()
    {
        // 현재 Z축 회전값 가져오기
        float zAngle = transform.eulerAngles.z;

        // 90도(위) 혹은 270도(-90도, 아래) 근처면 수직 방향
        // 수직이면 0.5, 수평(좌우)이면 2.0
        bool isVertical = Mathf.Abs(zAngle - 90f) < 1f || Mathf.Abs(zAngle - 270f) < 1f;
        float targetX = isVertical ? 0.5f : 2.0f;

        SetObjScaleX(obj_1, targetX);
        SetObjScaleX(obj_2, targetX);
    }

    private void SetObjScaleX(GameObject obj, float xSize)
    {
        if (obj != null)
        {
            Vector3 currentScale = obj.transform.localScale;
            currentScale.x = xSize;
            obj.transform.localScale = currentScale;
        }
    }

    private IEnumerator Co_SpawnProcess()
    {
        yield return new WaitForSeconds(1f);

        if (targetObject != null)
        {
            targetObject.SetActive(true);

            GameObject gas = targetObject;

            // [중요 2] 히트박스 찾기
            var hitbox = gas.GetComponent<AB_HitboxBase>();
            if (hitbox == null) hitbox = gas.GetComponentInChildren<AB_HitboxBase>();

            if (hitbox != null)
            {
                // [핵심] 중립 상태 설정
                hitbox.bMiddleState = true;

                // 소유자는 없음(null)으로 설정
                hitbox.Init(null);
            }
        }

        yield return new WaitForSeconds(1f);

        targetObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}