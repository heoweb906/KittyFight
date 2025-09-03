using UnityEngine;

public class PS_IQ20000 : Passive
{
    [Header("Cooldown")]
    [Tooltip("��� ��ų ��Ÿ�ӿ��� �� �� ���� ��ġ")]
    public float reduceBySeconds = 2f;

    private GameObject auraFxInstance;

    protected override void Subscribe(AbilityEvents e)
    {
        // ��Ÿ�� ��� ������ 2�� ����
        e.OnModifyCooldown += HandleModifyCooldown;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= HandleModifyCooldown;
    }

    private void HandleModifyCooldown(SkillType slot, ref float seconds)
    {
        if (reduceBySeconds <= 0f) return;
        seconds = Mathf.Max(0f, seconds - reduceBySeconds);
    }
}