using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [Header("카메라")]
    public GameObject objCamaera;

    [Header("초기 위치 저장용")]
    private Vector3 originPosition;
    private Vector3 originRotation;

    [Header("목표")]
    public Transform targetObject;               // 이동할 위치 기준
    public Vector3 targetRotationEuler_1;        // 회전 값 (왼쪽)
    public Vector3 targetRotationEuler_2;        // 회전 값 (오른쪽)
    public float moveDuration = 1.0f;


    private void Start()
    {
        SaveNowCameraSetting();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            MoveCameraToTarget(targetObject);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnToOrigin();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ShakeCamera();
        }
    }

    // 목표위치로 카메라를 이동, 회전 값은 위치에 따라 함수 내에서 설정됨
    public void MoveCameraToTarget(Transform trnasformTartget)
    {
        // Y축으로 5f 위로
        Vector3 targetPos = trnasformTartget.position + new Vector3(0, 5f, 0);
        Vector3 chosenRotation = trnasformTartget.position.x < 0 ? targetRotationEuler_1 : targetRotationEuler_2;

        objCamaera.transform.DOMove(targetPos, moveDuration)
            .SetEase(Ease.InOutSine);

        objCamaera.transform.DORotate(chosenRotation, moveDuration)
            .SetEase(Ease.InOutSine);
    }

    // 초기 위치로 카메라를 되돌리는 함수
    public void ReturnToOrigin()
    {
        objCamaera.transform.DOMove(originPosition, moveDuration)
            .SetEase(Ease.InOutSine);

        objCamaera.transform.DORotate(originRotation, moveDuration)
            .SetEase(Ease.InOutSine);
    }


    // 초기 카메라 위치 세팅
    private void SaveNowCameraSetting()
    {
        if (objCamaera != null)
        {
            originPosition = objCamaera.transform.position;
            originRotation = objCamaera.transform.eulerAngles;
        }
    }




    private Sequence shakeSequence;
    public void ShakeCamera(float strength = 0.3f, float duration = 0.2f)
    {
        if (objCamaera == null) return;

        // 낮은 진폭, Z축 고정, 덜덜 느낌
        Vector3 shakeStrength = new Vector3(strength, strength * 0.5f, 0f);

        // 기존 흔들림이 있으면 이어붙이기
        if (shakeSequence != null && shakeSequence.IsActive() && shakeSequence.IsPlaying())
        {
            shakeSequence.Append(
                objCamaera.transform.DOShakePosition(duration, shakeStrength, vibrato: 30, randomness: 3, snapping: false, fadeOut: true)
                    .SetEase(Ease.OutQuad)
            );
        }
        else
        {
            shakeSequence = DOTween.Sequence();
            shakeSequence.Append(
                objCamaera.transform.DOShakePosition(duration, shakeStrength, vibrato: 30, randomness: 3, snapping: false, fadeOut: true)
                    .SetEase(Ease.OutQuad)
            );
        }
    }
}
