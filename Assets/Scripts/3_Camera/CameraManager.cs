using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [Header("카메라")]
    public GameObject objCamaera;

    private Sequence shakeSequence;
    private Vector3 originalCameraPosition;


    private void Start()
    {
        if (objCamaera != null)
        {
            originalCameraPosition = objCamaera.transform.position;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShakeCameraPunch();
        }
    }


    public void ShakeCamera(float strength = 0.3f, float duration = 0.2f)
    {
        originalCameraPosition = objCamaera.transform.position;
        if (objCamaera == null) return;
        Vector3 shakeStrength = new Vector3(strength, strength * 0.5f, 0f);
        if (shakeSequence != null)
        {
            shakeSequence.Kill();
        }
        objCamaera.transform.DOKill();
        objCamaera.transform.position = originalCameraPosition;
        shakeSequence = DOTween.Sequence();
        shakeSequence.Append(
            objCamaera.transform.DOShakePosition(duration, shakeStrength, vibrato: 30, randomness: 3, snapping: false, fadeOut: true)
                .SetEase(Ease.OutQuad)
        );
    }

    public void ShakeCameraPunch(float strength = 0.2f, float duration = 0.6f, Vector3 direction = default)
    {
        if (objCamaera == null) return;

        // 기존 트윈 및 시퀀스 중지
        if (shakeSequence != null)
        {
            shakeSequence.Kill();
        }
        objCamaera.transform.DOKill();

        // 카메라 위치를 원래 위치로 리셋
        // (주의: originalCameraPosition이 Start에서 정확히 설정되었는지 확인 필수)
        objCamaera.transform.position = originalCameraPosition;

        shakeSequence = DOTween.Sequence();

        // 기본 방향을 우측으로 설정 (인수 없을 시 Vector3.right 사용)
        Vector3 punchVector = (direction == Vector3.zero) ? Vector3.right * strength : direction.normalized * strength;

        // DOPunchPosition 적용
        shakeSequence.Append(
            objCamaera.transform.DOPunchPosition(
                punchVector,
                duration,             // 지속 시간은 짧게 유지 (순간적인 튀웅 효과)
                vibrato: 10,          // 진동 횟수를 높여 스프링처럼 보이게 함
                elasticity: 0.6f        // 탄력성을 최대로 설정하여 강하게 반동
            )
            .SetEase(Ease.OutBack) // OutBack 이징 함수가 스프링 반동을 시각적으로 돕습니다.
        );
    }
}