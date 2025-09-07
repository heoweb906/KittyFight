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

    public void ShakeCamera(float strength = 0.3f, float duration = 0.2f)
    {
        if (objCamaera == null) return;

        Vector3 shakeStrength = new Vector3(strength, strength * 0.5f, 0f);

        if (shakeSequence != null)
        {
            shakeSequence.Kill();
        }

        objCamaera.transform.DOKill();

        shakeSequence = DOTween.Sequence();
        shakeSequence.Append(
            objCamaera.transform.DOShakePosition(duration, shakeStrength, vibrato: 30, randomness: 3, snapping: false, fadeOut: true)
                .SetEase(Ease.OutQuad)
        );
    }

    public void ShakeCameraPunch(float strength = 0.3f, float duration = 0.2f, Vector3 direction = default)
    {
        if (objCamaera == null) return;

        if (shakeSequence != null)
        {
            shakeSequence.Kill();
        }
        objCamaera.transform.DOKill();

        shakeSequence = DOTween.Sequence();

        if (direction != Vector3.zero)
        {
            // 방향성 있는 펀치 효과
            shakeSequence.Append(
                objCamaera.transform.DOPunchPosition(direction.normalized * strength, duration, vibrato: 20, elasticity: 0.3f)
                    .SetEase(Ease.OutQuad)
            );
        }
        else
        {
            // 기존 랜덤 흔들림
            Vector3 shakeStrength = new Vector3(strength, strength * 0.5f, 0f);
            shakeSequence.Append(
                objCamaera.transform.DOShakePosition(duration, shakeStrength, vibrato: 30, randomness: 3, snapping: false, fadeOut: true)
                    .SetEase(Ease.OutQuad)
            );
        }
    }
}