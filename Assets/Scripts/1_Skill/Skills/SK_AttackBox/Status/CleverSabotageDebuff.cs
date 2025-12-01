using UnityEngine;

[RequireComponent(typeof(PlayerAbility))]
public class CleverSabotageDebuff : MonoBehaviour
{
    private PlayerAbility ability;
    private AbilityEvents events;

    private float extraPerStack = 1.5f;
    private int stackCount = 0;
    private bool subscribed = false;

    public void Init(PlayerAbility targetAbility, float extraCooldownPerStack)
    {
        ability = targetAbility;
        extraPerStack = extraCooldownPerStack;

        if (ability != null)
        {
            events = ability.events;
            if (events == null)
            {
                events = ability.GetComponent<AbilityEvents>();
            }
        }
    }

    public void AddStack()
    {
        stackCount++;
        if (stackCount > 0 && !subscribed)
        {
            Subscribe();
        }
    }

    public void RemoveStack()
    {
        stackCount--;
        if (stackCount <= 0)
        {
            stackCount = 0;
            Unsubscribe();
            Destroy(this);
        }
    }

    private void Subscribe()
    {
        if (events == null || subscribed) return;
        events.OnModifyCooldown += OnModifyCooldown;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (events == null || !subscribed) return;
        events.OnModifyCooldown -= OnModifyCooldown;
        subscribed = false;
    }

    private void OnModifyCooldown(SkillType type, ref float duration)
    {
        if (stackCount <= 0) return;
        if (extraPerStack <= 0f) return;

        duration += extraPerStack * stackCount;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}