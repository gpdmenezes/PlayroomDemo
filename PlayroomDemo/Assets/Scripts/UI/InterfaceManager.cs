using UnityEngine;

namespace PlayroomDemo.UI
{
    public class InterfaceManager : MonoBehaviour
    {
        public static InterfaceManager Instance;

        [SerializeField] private Fader fader = null;
        [SerializeField] private PlayerInterface currentPlayerInterface = null;
        [SerializeField] private PlayerInterface opponentPlayerInterface = null;
        [SerializeField] private GameObject tutorialScreen = null;
        [SerializeField] private ResultScreen resultScreen = null;

        private void Awake ()
        {
            Instance = this;
            ResetInterface();
        }

        private void ResetInterface ()
        {
            fader.gameObject.SetActive(true);
            fader.ResetFader();
            tutorialScreen.gameObject.SetActive(true);
            resultScreen.gameObject.SetActive(false);
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

        public void SetDogCounter (int count)
        {
            currentPlayerInterface.SetDogCounterText(count);
            opponentPlayerInterface.SetDogCounterText(count);
        }

        public void SetWinner (bool isPlayer, bool isJaguar)
        {
            resultScreen.gameObject.SetActive(true);
            resultScreen.SetupScreen(isPlayer, isJaguar);
        }
    }
}