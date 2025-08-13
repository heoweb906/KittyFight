using UnityEngine;
using DG.Tweening;
using TMPro;

public class MapGimicChangeTest : MonoBehaviour
{
    [Header("Spin Settings")]
    [Tooltip("�� ���� �߰� ȸ������")]
    public int extraFullRotations = 4;
    [Tooltip("ȸ�� �� ���ӿ� �ɸ��� �ð�(��)")]
    public float spinDuration = 2f;
    [Tooltip("�ڽ��� 12��(��)�� �� ���� ���� Y�� ���� (���� 90��)")]
    public float topWorldAngle = 90f;

    [Header("UI")]
    [Tooltip("���õ� ������Ʈ �̸��� ǥ���� TMP �ؽ�Ʈ")]
    public TMP_Text textMeshPro;

    [Header("Effect Object")]
    [Tooltip("���� �Ϸ� �� Ȱ��ȭ�� ������Ʈ")]
    public GameObject obj;

    private bool isSpinning = false;

    void Update()
    {
         // if (Input.GetKeyDown(KeyCode.Y) && !isSpinning) StartSpin();

    }

    private void StartSpin()
    {
        int count = transform.childCount;
        if (count == 0) return;

        // 1) ���� ���� ��: ��� �ڽ� Ȱ��ȭ
        for (int i = 0; i < count; i++)
            transform.GetChild(i).gameObject.SetActive(true);

        // 2) �������� �� ĭ �̵����� ���� (0 ~ count-1)
        float angleStep = 360f / count;
        int randomSteps = Random.Range(0, count);

        // 3) ��ǥ ���� ���: ���� + ��ü ȸ�� + ���� ���� * ���� ����
        float currentZ = transform.eulerAngles.z;
        float targetZ = currentZ
                        + extraFullRotations * 360f
                        + randomSteps * angleStep;

        // 4) DOTween���� ȸ�� + ����
        isSpinning = true;
        transform
            .DORotate(new Vector3(0f, 0f, targetZ), spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(OnSpinComplete);
    }

    private void OnSpinComplete()
    {
        isSpinning = false;

        int count = transform.childCount;
        if (count == 0) return;

        // 1) 36�� ������ ����: ���� ����� �ܰ�� ����
        float angleStep = 360f / count;
        float snappedZ = Mathf.Round(transform.eulerAngles.z / angleStep) * angleStep;
        transform.rotation = Quaternion.Euler(0f, 0f, snappedZ);

        // 2) ���� Y��ǥ�� ���� ���� �ڽ� ã��
        Transform topChild = null;
        float highestY = float.MinValue;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            float worldY = child.position.y;
            if (worldY > highestY)
            {
                highestY = worldY;
                topChild = child;
            }
        }

        // 3) ������ �ڽ� ��Ȱ��ȭ, topChild�� ����
        for (int i = 0; i < count; i++)
            transform.GetChild(i).gameObject.SetActive(transform.GetChild(i) == topChild);

        // 4) �̸� ǥ��
        string nameToShow = topChild != null ? topChild.name : "None";
        Debug.Log($"���õ� ������Ʈ: {nameToShow}");
        if (textMeshPro != null)
            textMeshPro.text = nameToShow;

        // 5) ȿ�� ������Ʈ ��� (1�� ����)
        if (obj != null)
        {
            obj.SetActive(true);
            DOVirtual.DelayedCall(1f, () => obj.SetActive(false));
        }
    }
}
