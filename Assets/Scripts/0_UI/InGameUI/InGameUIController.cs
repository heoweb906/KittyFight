using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    [Header("스킬 카드 컨트롤러")]
    [SerializeField] SkillCardController skillCardController;

    private Canvas canvasMain;

    private void Awake()
    {
        canvasMain = FindObjectOfType<Canvas>();
        if (canvasMain == null)
        {
            Debug.LogError("[InGameUIController] Canvas가 씬에 없습니다.");
            return;
        }

        if (skillCardController == null) Debug.LogError("[InGameUIController] SkillCardController가 할당되지 않았습니다.");
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