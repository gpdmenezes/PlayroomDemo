using System.Collections.Generic;
using UnityEngine;
using Playroom;
using AOT;
using System;
using Random = UnityEngine.Random;
using static Playroom.PlayroomKit;
using PlayroomDemo.UI;
using PlayroomDemo.Board;

namespace PlayroomDemo.Networking
{
    public class PlayroomManager : MonoBehaviour
    {
        public static PlayroomManager Instance;

        [SerializeField] private static bool playerJoined;

        private PlayroomKit playroomKit = new();
        private static readonly List<Player> currentPlayers = new();
        private bool hasMatchStarted = false;
        private string playerRole = "none";
        private string playerTurn = "none";
        private Vector2 selectedPieceCoordinates = new Vector2(-1, -1);
        private Vector2 selectedPositionCoordinates = new Vector2(-1, -1);

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
                    {"selectedPieceCoordinates", new Vector2(-1, -1)},
                    {"selectedPositionCoordinates", new Vector2(-1, -1)},
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
        }

        private void Update ()
        {
            if (!playerJoined) return;
            if (!hasMatchStarted && currentPlayers.Count >= 2) StartMatch();
            if (!hasMatchStarted) return;

            CheckPlayerTurnUpdate(playroomKit.GetState<string>("playerTurn"));
            CheckSelectedPieceCoordinatesUpdate(playroomKit.GetState<Vector2>("selectedPieceCoordinates"));
            CheckSelectedPositionCoordinatesUpdate(playroomKit.GetState<Vector2>("selectedPositionCoordinates"));
        }

        public void StartMatch ()
        {
            Debug.Log("Starting Match...");
            hasMatchStarted = true;
            BoardManager.Instance.ResetBoard();
            ResetCoordinates();
            SetPlayerRoles();
        }

        private void ResetCoordinates ()
        {
            playroomKit.SetState("selectedPieceCoordinates", new Vector2(-1, -1), true);
            playroomKit.SetState("selectedPositionCoordinates", new Vector2(-1, -1), true);
        }

        private void SetPlayerRoles ()
        {
            if (playroomKit.IsHost())
            {
                playerRole = "player1";
            }
            else
            {
                playerRole = "player2";
                playroomKit.SetState("isPlayer2Ready", "true", true);
            }
        }

        private void ChooseJaguarPlayer()
        {
            Debug.Log("Choosing Jaguar Player...");
            int randomJaguarSelection = Random.Range(0, 2);
            string player = randomJaguarSelection == 0 ? "player1" : "player2";
            playroomKit.SetState("jaguarPlayer", player, true);
            playroomKit.SetState("playerTurn", player, true);
        }

        public void OnJaguarPlayerChosen (string jaguarPlayer)
        {
            bool amIJaguar = (playerRole == jaguarPlayer);
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

        private void CheckPlayerTurnUpdate (string playerTurn)
        {
            if (this.playerTurn == playerTurn) return;
            Debug.Log("PlayerTurn updated: " + playerTurn);

            this.playerTurn = playerTurn;
            bool isCurrentPlayerTurn = (playerTurn == playerRole);
            InterfaceManager.Instance.SetPlayerTurnText(isCurrentPlayerTurn);
            PlayerController.Instance.SetPlayerTurn(isCurrentPlayerTurn);
        }

        private void CheckSelectedPieceCoordinatesUpdate (Vector2 selectedPieceCoordinates)
        {
            if (selectedPieceCoordinates == null || this.selectedPieceCoordinates == selectedPieceCoordinates) return;
            Debug.Log("SelectedPieceCoordinates updated: " + selectedPieceCoordinates);

            BoardPiece oldPiece = BoardManager.Instance.GetBoardPieceByCoordinate(this.selectedPieceCoordinates);
            if (oldPiece != null) oldPiece.OnInteraction(false);

            this.selectedPieceCoordinates = selectedPieceCoordinates;
            BoardPiece selectedPiece = BoardManager.Instance.GetBoardPieceByCoordinate(selectedPieceCoordinates);
            if (selectedPiece != null) selectedPiece.OnInteraction(true);
        }

        private void CheckSelectedPositionCoordinatesUpdate (Vector2 selectedPositionCoordinates)
        {
            if (selectedPositionCoordinates == null || this.selectedPositionCoordinates == selectedPositionCoordinates) return;
            Debug.Log("SelectedPositionCoordinates updated: " + selectedPositionCoordinates);
            
            this.selectedPositionCoordinates = selectedPositionCoordinates;
            BoardPiece selectedPiece = BoardManager.Instance.GetBoardPieceByCoordinate(selectedPieceCoordinates);
            BoardPosition selectedPosition = BoardManager.Instance.GetBoardPositionByCoordinate(selectedPositionCoordinates);
            PlayerController.Instance.SetReceivedMove(selectedPiece, selectedPosition);
            selectedPiece.OnInteraction(false);
        }

        public void OnPlayerFinishedTurn ()
        {
            playroomKit.SetState("playerTurn", (playerTurn == "player1") ? "player2" : "player1", true);
        }

        public void OnPlayerSelectedPiece (Vector2 selectedPieceCoordinates)
        {
            playroomKit.SetState("selectedPieceCoordinates", selectedPieceCoordinates, true);
        }

        public void OnPlayerSelectedPosition (Vector2 selectedPositionCoordinates)
        {
            playroomKit.SetState("selectedPositionCoordinates", selectedPositionCoordinates, true);
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