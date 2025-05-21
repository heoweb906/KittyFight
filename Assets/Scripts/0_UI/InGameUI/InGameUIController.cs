using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    [Header("��ų ī�� ��Ʈ�ѷ�")]
    [SerializeField] SkillCardController skillCardController;

    private Canvas canvasMain;

    private void Awake()
    {
        canvasMain = FindObjectOfType<Canvas>();
        if (canvasMain == null)
        {
            Debug.LogError("[InGameUIController] Canvas�� ���� �����ϴ�.");
            return;
        }

        if (skillCardController == null) Debug.LogError("[InGameUIController] SkillCardController�� �Ҵ���� �ʾҽ��ϴ�.");
        else skillCardController.Initialize(this, canvasMain.transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            skillCardController.ShowAll();
        if (Input.GetKeyDown(KeyCode.R))
            skillCardController.HideAll();
    }
}