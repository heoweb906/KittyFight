using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [Header("ī�޶�")]
    public GameObject objCamaera;

    [Header("�ʱ� ��ġ �����")]
    private Vector3 originPosition;
    private Vector3 originRotation;

    [Header("��ǥ")]
    public Transform targetObject;               // �̵��� ��ġ ����
    public Vector3 targetRotationEuler_1;        // ȸ�� �� (����)
    public Vector3 targetRotationEuler_2;        // ȸ�� �� (������)
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

    // ��ǥ��ġ�� ī�޶� �̵�, ȸ�� ���� ��ġ�� ���� �Լ� ������ ������
    public void MoveCameraToTarget(Transform trnasformTartget)
    {
        // Y������ 5f ����
        Vector3 targetPos = trnasformTartget.position + new Vector3(0, 5f, 0);
        Vector3 chosenRotation = trnasformTartget.position.x < 0 ? targetRotationEuler_1 : targetRotationEuler_2;

        objCamaera.transform.DOMove(targetPos, moveDuration)
            .SetEase(Ease.InOutSine);

        objCamaera.transform.DORotate(chosenRotation, moveDuration)
            .SetEase(Ease.InOutSine);
    }

    // �ʱ� ��ġ�� ī�޶� �ǵ����� �Լ�
    public void ReturnToOrigin()
    {
        objCamaera.transform.DOMove(originPosition, moveDuration)
            .SetEase(Ease.InOutSine);

        objCamaera.transform.DORotate(originRotation, moveDuration)
            .SetEase(Ease.InOutSine);
    }


    // �ʱ� ī�޶� ��ġ ����
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

        // ���� ����, Z�� ����, ���� ����
        Vector3 shakeStrength = new Vector3(strength, strength * 0.5f, 0f);

        // ���� ��鸲�� ������ �̾���̱�
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
