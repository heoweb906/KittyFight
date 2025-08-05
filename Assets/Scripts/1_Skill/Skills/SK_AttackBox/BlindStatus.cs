using System.Collections;
using UnityEngine;

public class BlindStatus : MonoBehaviour
{
    public void ApplyBlind(float duration)
    {
        InGameUIController.Instance?.ShowBlindOverlay(duration);
        StartCoroutine(RemoveAfterDelay(duration));
    }

    private IEnumerator RemoveAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(this); // 상태 컴포넌트 제거
    }
}