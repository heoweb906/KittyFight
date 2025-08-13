using UnityEngine;

public class SkillExecuteHandler : IP2PMessageHandler
{
    private readonly PlayerAbility opponentAbility;
    private readonly int myPlayerNumber;

    public SkillExecuteHandler(PlayerAbility opponentAbility, int myPlayerNumber)
    {
        this.opponentAbility = opponentAbility;
        this.myPlayerNumber = myPlayerNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith(SkillMessageBuilder.Prefix);

    public void Handle(string msg)
    {
        if (opponentAbility == null) return;

        var json = msg.Substring(SkillMessageBuilder.Prefix.Length);
        var data = JsonUtility.FromJson<SkillExecuteMessage>(json);
        if (data == null) return;

        // 내가 보낸 건 무시(루프 방지)
        if (data.player == myPlayerNumber) return;

        Vector3 origin = new Vector3(data.ox, data.oy, data.oz);
        Vector3 dir = new Vector3(data.dx, data.dy, data.dz);
        var type = (SkillType)data.skillType;

        // 원격 기원: 강제 실행(쿨타임/조건 동일 적용)
        opponentAbility.TryExecuteSkill(type, origin, dir, force: true);
    }
}