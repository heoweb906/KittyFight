using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [Header("Ä«¸Þ¶ó")]
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

    public void ShakeCameraPunch(float strength = 0.1f, float duration = 0.1f, Vector3 direction = default)
    {
        if (objCamaera == null) return;
        if (shakeSequence != null)
        {
            shakeSequence.Kill();
        }
        objCamaera.transform.DOKill();
        objCamaera.transform.position = originalCameraPosition;
        shakeSequence = DOTween.Sequence();
        if (direction != Vector3.zero)
        {
            shakeSequence.Append(
                objCamaera.transform.DOPunchPosition(direction.normalized * strength, duration, vibrato: 30, elasticity: 1f)
                    .SetEase(Ease.OutBack)
            );
        }
        else
        {
            Vector3 shakeStrength = new Vector3(strength, strength * 0.5f, 0f);
            shakeSequence.Append(
                objCamaera.transform.DOShakePosition(duration, shakeStrength, vibrato: 30, randomness: 90, snapping: false, fadeOut: true)
                    .SetEase(Ease.InOutSine)
            );
        }
    }
}