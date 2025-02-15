using UnityEngine;
using UnityEngine.UI;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] private Image playerBackground = null;
    [SerializeField] private Text playerName = null;
    [SerializeField] private Text playerType = null;
    [SerializeField] private Text playerTurn = null;

    public void SetupInterface (bool isJaguar, string playerName)
    {
        this.playerName.text = playerName;
        if (isJaguar)
        {
            playerType.text = "Jaguar";
            playerBackground.color = Color.yellow;
        }
        else
        {
            playerType.text = "Dogs";
            playerBackground.color = Color.green;
        }
    }

    public void SetPlayerTurnText (bool isMyTurn)
    {
        if (isMyTurn)
        {
            playerTurn.text = "MY TURN";
        }
        else
        {
            playerTurn.text = "";
        }
    }
}
