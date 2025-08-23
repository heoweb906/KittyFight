using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [Header("Ä«¸Þ¶ó")]
    public GameObject objCamaera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShakeCamera();
        }
    }

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
}