using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerHealthUI hpUI;
    [SerializeField] private SkillCooldownUI skillUI;

    public PlayerHealthUI GetHealthUI() => hpUI;
    public SkillCooldownUI GetSkillUI() => skillUI;
}