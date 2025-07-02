using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class InGameUIController : MonoBehaviour 
{
    private Canvas canvasMain;
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

        //}

    }

}