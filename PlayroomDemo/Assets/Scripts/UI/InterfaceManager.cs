using UnityEngine;

namespace PlayroomDemo.UI
{
    public class InterfaceManager : MonoBehaviour
    {
        public static InterfaceManager Instance;

        [SerializeField] private Fader fader;
        [SerializeField] private PlayerInterface currentPlayerInterface;
        [SerializeField] private PlayerInterface opponentPlayerInterface;

        private void Awake ()
        {
            Instance = this;
            fader.gameObject.SetActive(true);
        }

        public void SetupCurrentPlayerInterface (bool isJaguar, string playerName)
        {
            Debug.Log("CurrentPlayer: " + playerName + " /isJaguar: " + isJaguar);
            currentPlayerInterface.SetupInterface(isJaguar, playerName);
        }

        public void SetupOpponentPlayerInterface (bool isJaguar, string playerName)
        {
            Debug.Log("OpponentPlayer: " + playerName + " /isJaguar: " + isJaguar);
            opponentPlayerInterface.SetupInterface(isJaguar, playerName);
        }

        public void FadeOut ()
        {
            fader.FadeOut();
        }

        public void SetPlayerTurnText (bool isCurrentPlayerTurn)
        {
            currentPlayerInterface.SetPlayerTurnText(isCurrentPlayerTurn);
            opponentPlayerInterface.SetPlayerTurnText(!isCurrentPlayerTurn);
        }
    }
}