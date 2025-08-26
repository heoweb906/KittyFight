using UnityEngine;

public abstract class Passive : MonoBehaviour
{
    protected PlayerAbility ability;
    protected AbilityEvents events;

    public virtual void OnEquip(PlayerAbility a)
    {
        ability = a;
        events = a.events;
        events = a.GetComponent<AbilityEvents>();
        if (events == null) events = a.gameObject.AddComponent<AbilityEvents>();
        Subscribe(events);
    }

    public virtual void OnUnequip()
    {
        if (events != null) Unsubscribe(events);
        events = null; ability = null;
    }

    protected virtual void Subscribe(AbilityEvents e) { }
    protected virtual void Unsubscribe(AbilityEvents e) { }
}