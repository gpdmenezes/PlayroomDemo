using System.Collections.Generic;
using UnityEngine;
using Playroom;
using AOT;
using System;
using Random = UnityEngine.Random;
using static Playroom.PlayroomKit;

namespace PlayroomDemo
{
    public class PlayroomManager : MonoBehaviour
    {
        public static PlayroomManager Instance;

        [SerializeField] private static bool playerJoined;

        private PlayroomKit playroomKit = new();
        private static readonly List<Player> currentPlayers = new();
        private string playerRole = "none";
        private string playerTurn = "none";
        private bool hasMatchStarted = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            playroomKit.InsertCoin(new InitOptions()
            {
                allowGamepads = false,
                skipLobby = false,
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() {
                    {"isPlayer2Ready", "false"},
                    {"jaguarPlayer", "none"},
                    {"playerTurn", "none"},
            },
            }, () => {
                playroomKit.OnPlayerJoin(AddPlayer);
            });

            playroomKit.WaitForState("isPlayer2Ready", (value) =>
            {
                string receivedValue = playroomKit.GetState<string>("isPlayer2Ready");
                if (playroomKit.IsHost()) ChooseJaguarPlayer();
            });

            playroomKit.WaitForState("jaguarPlayer", (value) => 
            {
                string receivedValue = playroomKit.GetState<string>("jaguarPlayer");
                OnJaguarPlayerChosen(receivedValue);
            });

            playroomKit.WaitForState("playerTurn", (value) =>
            {
                string receivedValue = playroomKit.GetState<string>("playerTurn");
                OnPlayerTurnUpdate(receivedValue);
            });
        }

        private void Update ()
        {
            if (!playerJoined) return;
            if (!hasMatchStarted && currentPlayers.Count >= 2)
            {
                hasMatchStarted = true;
                StartMatch();
            }
        }

        public void StartMatch ()
        {
            Debug.Log("Starting Match!");
            BoardManager.Instance.ResetBoard();
            SetPlayerRoles();
        }

        private void SetPlayerRoles ()
        {
            if (playroomKit.IsHost())
            {
                Debug.Log(playroomKit.MyPlayer().GetProfile().name + " IS HOST AND PLAYER1");
                playerRole = "player1";
            }
            else
            {
                Debug.Log(playroomKit.MyPlayer().GetProfile().name + " IS CLIENT AND PLAYER2");
                playerRole = "player2";
                playroomKit.SetState("isPlayer2Ready", "true", true);
            }
        }

        private void ChooseJaguarPlayer()
        {
            Debug.Log("Choosing Jaguar Player");
            int randomJaguarSelection = Random.Range(0, 2);
            string player = randomJaguarSelection == 0 ? "player1" : "player2";
            playroomKit.SetState("jaguarPlayer", player, true);
            playroomKit.SetState("playerTurn", player, true);
        }

        public void OnJaguarPlayerChosen (string jaguarPlayer)
        {
            Debug.Log("OnJaguarPlayerChosen: " + jaguarPlayer + " / PlayerRole: " + playerRole);
            bool amIJaguar = (playerRole == jaguarPlayer);
            Debug.Log(playroomKit.MyPlayer().GetProfile().name + " / amIJaguar: " + amIJaguar);
            InterfaceManager.Instance.SetupCurrentPlayerInterface(amIJaguar, playroomKit.MyPlayer().GetProfile().name);
            InterfaceManager.Instance.SetupOpponentPlayerInterface(!amIJaguar, GetOtherPlayerName());
            PlayerController.Instance.SetPlayerJaguar(amIJaguar);
        }

        private string GetOtherPlayerName ()
        {
            string name = "";
            foreach (Player player in currentPlayers)
            {
                if (player == playroomKit.MyPlayer()) continue;
                name = player.GetProfile().name;
            }
            return name;
        }

        public void OnPlayerTurnUpdate (string playerTurn)
        {
            Debug.Log("OnPlayerTurnUpdate: " + playerTurn);
            this.playerTurn = playerTurn;
            bool isCurrentPlayerTurn = (playerTurn == playerRole);
            InterfaceManager.Instance.SetPlayerTurnText(isCurrentPlayerTurn);
            PlayerController.Instance.SetPlayerTurn(isCurrentPlayerTurn);
        }

        public void OnPlayerFinishedTurn ()
        {
            Debug.Log("Turn Finished.");
            if (playerTurn == "player1")
            {
                playroomKit.SetState("playerTurn", "player2", true);
            }
            else
            {
                playroomKit.SetState("playerTurn", "player1", true);
            }
        }

        public static void AddPlayer (Player player)
        {
            currentPlayers.Add(player);
            Debug.Log(player.GetProfile().name + " joined the game! (PlayerID: " + player.id + ")");
            playerJoined = true;
            player.OnQuit(RemovePlayer);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void RemovePlayer (string playerID)
        {
            Debug.Log("PlayerID " + playerID + " left the game.");
        }
    }

}