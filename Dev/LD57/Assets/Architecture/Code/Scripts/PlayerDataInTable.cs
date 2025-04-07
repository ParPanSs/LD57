using TMPro;
using UnityEngine;

public class PlayerDataInTable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerTablePlace;    
    [SerializeField] private TextMeshProUGUI playerName;    
    [SerializeField] private TextMeshProUGUI playerScore;    
    [SerializeField] private TextMeshProUGUI playerFish;

    public void SetPlayersTable(string playerTablePlace, string playerName, string playerScore, string playerFish)
    {
        this.playerTablePlace.text = playerTablePlace;
        this.playerName.text = playerName;
        this.playerScore.text = playerScore;
        this.playerFish.text = playerFish;
    }
}
