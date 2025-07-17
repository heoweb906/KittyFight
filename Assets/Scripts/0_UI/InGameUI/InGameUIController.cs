using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class InGameUIController : MonoBehaviour
{
    public static InGameUIController Instance { get; private set; }

    private Canvas canvasMain;
    public GameManager gameManager;

    public PlayerHealthUI hpUI_Player1;
    public SkillCooldownUI skillUI_Player1;
    public SkillCooldownUI skillUI2_Player1;

    public PlayerHealthUI hpUI_Player2;
    public SkillCooldownUI skillUI_Player2;
    public SkillCooldownUI skillUI2_Player2;
    public GameTimer gameTimer;

    public SkillCardController skillCardController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        canvasMain = FindObjectOfType<Canvas>();
        if (canvasMain == null) return;

       
        skillCardController.Initialize(this, canvasMain.transform);
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && MatchResultStore.myPlayerNumber == 2)
        {
            skillCardController.ShowSkillCardList(2);
            P2PMessageSender.SendMessage(
                BasicBuilder.Build(MatchResultStore.myPlayerNumber, "[SKILL_SHOW]"));

        }

    }

    public void InitializeHP(int playerNum, int maxHP)
    {
        if (playerNum == 1) hpUI_Player1?.Initialize(maxHP);
        else hpUI_Player2?.Initialize(maxHP);
    }

    public void UpdateHP(int playerNum, int hp)
    {
        if (playerNum == 1) hpUI_Player1?.SetHP(hp);
        else hpUI_Player2?.SetHP(hp);
    }

    public void StartSkillCooldown(int playerNum, int skill)
    {
        if(skill == 1)
        {
            if (playerNum == 1) skillUI_Player1?.StartCooldown();
            else skillUI_Player2?.StartCooldown();
        }
        else
        {
            if (playerNum == 1) skillUI2_Player1?.StartCooldown();
            else skillUI2_Player2?.StartCooldown();
        }
    }

    public void StartGameTimer(float duration)
    {
        gameTimer?.SetDuration(duration);
    }

    public void TickGameTimer()
    {
        if (gameTimer != null && gameTimer.Tick(Time.deltaTime))
        {
            GameObject.FindObjectOfType<GameManager>()?.EndGame();
        }
    }
}