using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour 
{
    [Header("UI Conrollers")]   // 다양한 기능을 가지고 있는 각각의 UI들을 개별로 관리할 거임
    [SerializeField] SkillCardController skillCardController;

    private Canvas canvasMain;

    private void Awake()
    {
        canvasMain = FindObjectOfType<Canvas>();
        if (canvasMain == null) return;

        if (skillCardController == null) Debug.LogError("[InGameUIController] SkillCardController�� �Ҵ���� �ʾҽ��ϴ�.");
        else skillCardController.Initialize(this, canvasMain.transform);
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            skillCardController.ShowSkillCardList();
        if (Input.GetKeyDown(KeyCode.R))
            skillCardController.HideSkillCardList();
    }

}