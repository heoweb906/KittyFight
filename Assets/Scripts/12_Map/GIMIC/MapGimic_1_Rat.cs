using UnityEngine;

public class MapGimic_1_Rat : AbstractMapGimic
{
    [Header("Gimmick Settings")]
    [SerializeField] private float activationInterval = 3.0f;
    private float timer;

    public override void OnGimicStart()
    {
        base.OnGimicStart();
        timer = 0f;
        Debug.Log("[Rat] 쥐 기믹 시작 (Host만 로직 수행)");
    }

    public override void OnGimmickUpdate()
    {
        // 1. [테스트용] 평소 상태 깜빡임 (빨강 <-> 흰색)
        flashTimer += Time.deltaTime;
        if (flashTimer >= 0.5f)
        {
            flashTimer = 0f;
            isWhitePhase = !isWhitePhase;

            if (mapManager != null && mapManager.iamge_TestMapGimicColor != null)
            {
                // 평소엔 빨간색 계열로 깜빡임
                mapManager.iamge_TestMapGimicColor.color = isWhitePhase ? Color.white : Color.red;
            }
        }

        // 2. [핵심 로직] 호스트(P1)만 타이머 체크
        if (MatchResultStore.myPlayerNumber == 1)
        {
            timer += Time.deltaTime;

            if (timer >= activationInterval)
            {
                // ★ 발동 시각적 피드백 (파란색!)
                if (mapManager != null && mapManager.iamge_TestMapGimicColor != null)
                    mapManager.iamge_TestMapGimicColor.color = Color.blue;

                Debug.Log($"[Rat] 타이머 {activationInterval}초 달성! 체력 분배 시도...");

                DistributeHealth_Host();
                timer = 0f;
            }
        }
    }

    // [Host 전용]
    private void DistributeHealth_Host()
    {
        if (!gameManager.playerAbility_1 || !gameManager.playerAbility_2)
        {
            Debug.LogError("[Rat] 실패: 플레이어 참조가 없음");
            return;
        }

        var p1Health = gameManager.playerAbility_1.GetComponent<PlayerHealth>();
        var p2Health = gameManager.playerAbility_2.GetComponent<PlayerHealth>();

        if (p1Health == null || p2Health == null) return;

        // 1. 계산
        int hp1 = p1Health.CurrentHP;
        int hp2 = p2Health.CurrentHP;
        int totalHP = hp1 + hp2;
        int averageHP = totalHP / 2;

        Debug.Log($"[Rat] 계산: P1({hp1}) + P2({hp2}) = {totalHP} / 2 => 목표 {averageHP}");

        // 변동 없으면 패스
        if (hp1 == averageHP && hp2 == averageHP)
        {
            Debug.Log("[Rat] 이미 평균값이므로 체력 변동 없음 (Pass)");
            return;
        }

        // 2. 내꺼(P1) 적용
        ApplyHP(p1Health, averageHP);

        // 3. 패킷 전송 (주석 해제 권장)
        string packet = MapGimicBuilder.BuildRat_SyncHP(gameManager.mapManager.GetMapGimicIndex(), averageHP);
        P2PMessageSender.SendMessage(packet); 
    }

    public void ReceiveSyncHP(int targetHP)
    {
        var p2Health = gameManager.playerAbility_2.GetComponent<PlayerHealth>();
        if (p2Health != null)
        {
            ApplyHP(p2Health, targetHP);
        }
    }

    private void ApplyHP(PlayerHealth target, int targetValue)
    {
        int current = target.CurrentHP;
        int diff = current - targetValue;

        if (diff > 0)
        {
            Debug.Log($"[Rat] {target.name} 데미지: {diff}");
            target.ForceDamage(diff);
        }
        else if (diff < 0)
        {
            Debug.Log($"[Rat] {target.name} 힐: {Mathf.Abs(diff)}");
            target.Heal(Mathf.Abs(diff));
        }
    }
}