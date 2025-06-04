using UnityEngine;

[RequireComponent(typeof(PlayerAbility))]
public class PlayerAbilityLoader : MonoBehaviour
{
    public PlayerAbilityData player1Data;
    public PlayerAbilityData player2Data;

    void Awake()
    {
        PlayerAbility ability = GetComponent<PlayerAbility>();
        int num = MatchResultStore.myPlayerNumber;

        if (num == 1)
        {
            ability.abilityData = player1Data;
        }
        else if (num == 2)
        {
            ability.abilityData = player2Data;
        }
        else
        {
            Debug.LogWarning("Unknown player number. Using default (Player1Data)");
            ability.abilityData = player1Data;
        }
    }
}