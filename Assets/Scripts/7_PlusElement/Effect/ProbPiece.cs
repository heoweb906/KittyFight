using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbPiece : MonoBehaviour
{
    public float fDisspearDelay;
    public float fDissapearDuration;
   


    public void OnThisPiece()
    {
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("IgnorePlayer"));

        DOVirtual.DelayedCall(fDisspearDelay, () =>
        {
            if (this != null && gameObject != null)
            {
                transform.DOScale(Vector3.zero, fDissapearDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => {
                        if (this != null && gameObject != null)
                        {
                            Destroy(gameObject);
                        }
                    });
            }
        });
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
