using UnityEngine;

public class P2PSkillHandler : IP2PMessageHandler
{
    private readonly SkillWorker skillWorker;
    private readonly int myPlayerNumber;

    public P2PSkillHandler(SkillWorker worker, int myNumber)
    {
        skillWorker = worker;
        myPlayerNumber = myNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[SKILL]");

    public void Handle(string msg)
    {
        string json = msg.Substring(7);
        var model = JsonUtility.FromJson<Model_Skill>(json);

        if (model.player == myPlayerNumber)
            return;

        try
        {
            if (string.IsNullOrEmpty(model.slot))
            {
                Debug.LogError($"[P2PSkillHandler] slot 값이 비어있음. 받은 데이터: {json}");
                return;
            }

            SkillSlotType slotType = (SkillSlotType)System.Enum.Parse(
                typeof(SkillSlotType), model.slot, true
            );

            // model.skillName을 이용해 SkillCard_SO를 찾아서 넘겨주도록 수정
            SkillCard_SO card = skillWorker.skillCards.Find(c => c.sSkillName == model.skillName);
            if (card == null)
            {
                Debug.LogError($"[P2PSkillHandler] '{model.skillName}'에 해당하는 SkillCard_SO를 찾을 수 없습니다.");
                return;
            }

            skillWorker.EquipSkillByCard(slotType, card);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[P2PSkillHandler] Skill 장착 중 오류 발생! slot: '{model.slot}', skillName: '{model.skillName}', 예외: {e}\n원본 데이터: {json}");
        }
    }
}
