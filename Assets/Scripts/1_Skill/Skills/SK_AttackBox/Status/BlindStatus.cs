using System.Collections;
using UnityEngine;

public class BlindStatus : MonoBehaviour
{
    public void ApplyBlind(float duration)
    {
        InGameUIController.Instance?.ShowBlindEggs(duration, 3);
        StartCoroutine(RemoveAfterDelay(duration));
    }

    private IEnumerator RemoveAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(this);
    }
}