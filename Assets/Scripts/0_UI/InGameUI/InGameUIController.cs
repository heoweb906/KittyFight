using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class InGameUIController : MonoBehaviour 
{
    public Canvas canvasMain;
    public GameManager gameManager;

    public SkillCardController skillCardController;

    private void Awake()
    {
        canvasMain = FindObjectOfType<Canvas>();
        if (canvasMain == null) return;

       
        skillCardController.Initialize(this, canvasMain.transform);
    }



    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y) && MatchResultStore.myPlayerNumber == 2)
        //{
        //    skillCardController.ShowSkillCardList(2);
        //    P2PMessageSender.SendMessage(
        //        BasicBuilder.Build(MatchResultStore.myPlayerNumber, "[SKILL_SHOW]"));


        //    Debug.Log("이게 작동하고 있습니다.");

        //}

        if (Input.GetKeyDown(KeyCode.Y))
        {
            skillCardController.ShowSkillCardList(2);

            Debug.Log("스킬 카드 보이게 하기");
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            skillCardController.HideSkillCardList();

            Debug.Log("스킬 카드 숨기기");
        }

    }

}