using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class MainMenuController : MonoBehaviour
{
    public List<GameObject> panels; // �ε����� ����
    private int currentIndex = -1;

    void Start()
    {
        // ��� �г� ���ֱ�
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        // 0�� �г� Ȱ��ȭ
        if (panels.Count > 0)
        {
            currentIndex = 0;
            panels[0].SetActive(true);

            // ���İ� 1�� �ʱ�ȭ
            Graphic[] graphics = panels[0].GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                Color c = g.color;
                g.color = new Color(c.r, c.g, c.b, 1f);
            }
        }
    }



    // #. �ε巯�� �г� ��ü
    public void SwitchPanel_ByButton(int targetIndex) // ��ư�� �Լ�
    {
        if (targetIndex < 0 || targetIndex >= panels.Count || targetIndex == currentIndex)
            return;
        StartCoroutine(SwitchRoutine(targetIndex, 0.1f));
    }

    private IEnumerator SwitchRoutine(int targetIndex, float fDuration)
    {
        GameObject currentPanel = currentIndex >= 0 ? panels[currentIndex] : null;
        GameObject nextPanel = panels[targetIndex];

        if (currentPanel != null)
        {
            yield return StartCoroutine(FadeOutPanel(currentPanel, fDuration));
        }

        nextPanel.SetActive(true);
        yield return StartCoroutine(FadeInPanel(nextPanel, fDuration));

        currentIndex = targetIndex;
    }

    private IEnumerator FadeOutPanel(GameObject panel, float fDuration)
    {
        Graphic[] graphics = panel.GetComponentsInChildren<Graphic>(true);
        foreach (var g in graphics)
        {
            g.DOFade(0f, fDuration);
        }

        yield return new WaitForSeconds(fDuration);
        panel.SetActive(false);
    }
    private IEnumerator FadeInPanel(GameObject panel, float fDuration)
    {
        Graphic[] graphics = panel.GetComponentsInChildren<Graphic>(true);
        foreach (var g in graphics)
        {
            Color c = g.color;
            g.color = new Color(c.r, c.g, c.b, 0f);
            g.DOFade(1f, fDuration);
        }

        yield return null;
    }


}
