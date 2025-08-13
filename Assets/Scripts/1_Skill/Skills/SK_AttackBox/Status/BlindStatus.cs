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
        Destroy(this); // ���� ������Ʈ ����
    }
}