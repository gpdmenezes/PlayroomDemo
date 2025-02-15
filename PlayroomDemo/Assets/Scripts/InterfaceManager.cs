using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    [SerializeField] private PlayerInterface player1Interface;
    [SerializeField] private PlayerInterface player2Interface;

    private void Awake ()
    {
        Instance = this;
    }

    public void SetupPlayerInterface (bool isPlayer1, string playerName, bool isJaguar)
    {
        if (isPlayer1)
        {
            player1Interface.SetupInterface(isJaguar, playerName);
        }
        else
        {
            player2Interface.SetupInterface(isJaguar, playerName);
        }
    }

    public void SetPlayerTurnText (bool isPlayer1Turn)
    {
        player1Interface.SetPlayerTurnText(isPlayer1Turn);
        player2Interface.SetPlayerTurnText(!isPlayer1Turn);
    }

}
