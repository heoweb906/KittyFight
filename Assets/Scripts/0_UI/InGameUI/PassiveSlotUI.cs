using UnityEngine;
using UnityEngine.UI;

public class PassiveSlotUI : MonoBehaviour
{
    [Header("참조")]
    public PlayerAbility ability;
    [Tooltip("패시브 슬롯 인덱스 (0, 1, 2)")]
    public int slotIndex = 0;

    [Header("이미지")]
    public Image iconImage;

    [Header("아이콘/배경 연출")]
    public Sprite emptySprite;
    [Range(0f, 1f)] public float iconAlphaWhenOn = 1f;
    [Range(0f, 1f)] public float iconAlphaWhenOff = 0f;

    private bool isSubscribed = false;

    private void OnEnable()
    {
        TrySubscribe();
        RefreshFromCurrentState();
    }

    private void OnDisable()
    {
        TryUnsubscribe();
    }

    public void Bind(PlayerAbility ability, int slotIndex)
    {
        TryUnsubscribe();

        this.ability = ability;
        this.slotIndex = slotIndex;

        if (!isActiveAndEnabled)
        {
            Apply(null);
            return;
        }

        TrySubscribe();
        RefreshFromCurrentState();
    }
    private void TrySubscribe()
    {
        if (ability != null && !isSubscribed)
        {
            ability.OnPassiveSlotChanged += HandlePassiveSlotChanged;
            isSubscribed = true;
        }
    }

    private void TryUnsubscribe()
    {
        if (ability != null && isSubscribed)
        {
            ability.OnPassiveSlotChanged -= HandlePassiveSlotChanged;
            isSubscribed = false;
        }
    }

    private void HandlePassiveSlotChanged(int index, Passive p)
    {
        if (index != slotIndex) return;
        Apply(p);
    }

    private void RefreshFromCurrentState()
    {
        if (ability == null)
        {
            Apply(null);
            return;
        }

        Passive p = null;
        if (slotIndex >= 0 && slotIndex < ability.passives.Count)
            p = ability.passives[slotIndex];

        Apply(p);
    }

    private void Apply(Passive p)
    {
        if (iconImage == null) return;

        var c = iconImage.color;

        if (p != null)
        {
            var icon = p.skillIcon;

            if (icon != null)
            {
                iconImage.sprite = icon;
                c.a = iconAlphaWhenOn;
                iconImage.color = c;

                iconImage.enabled = true;
                return;
            }
        }

        iconImage.sprite = emptySprite;
        c.a = iconAlphaWhenOff;
        iconImage.color = c;
        iconImage.enabled = (emptySprite != null);
    }
}