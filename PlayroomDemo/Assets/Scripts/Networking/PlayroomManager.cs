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
        private bool amIJaguar = false;
        private string playerTurn = "none";
        private Vector2 selectedPieceCoordinates = new Vector2(-1, -1);
        private Vector2 selectedPositionCoordinates = new Vector2(-1, -1);
        private string winner = "none";
        private int dogCounter = 0;

        private void Awake ()
        {
            Instance = this;
        }

        private void Start ()
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
                    {"winner", "none" },
                    {"dogCounter", 0 },
            },
            }, () => {
                playroomKit.OnPlayerJoin(AddPlayer);
            });

            playroomKit.WaitForState("isPlayer2Ready", (value) =>
            {
                string receivedValue = playroomKit.GetState<string>("isPlayer2Ready");
                ChooseJaguarPlayer();
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

            CheckWinnerUpdate(playroomKit.GetState<string>("winner"));
            CheckDogCounterUpdate(playroomKit.GetState<int>("dogCount"));
            CheckPlayerTurnUpdate(playroomKit.GetState<string>("playerTurn"));
            CheckSelectedPieceCoordinatesUpdate(playroomKit.GetState<Vector2>("selectedPieceCoordinates"));
            CheckSelectedPositionCoordinatesUpdate(playroomKit.GetState<Vector2>("selectedPositionCoordinates"));
        }

        private void StartMatch ()
        {
            Debug.Log("Starting Match...");
            hasMatchStarted = true;
            BoardManager.Instance.ResetBoard();
            ResetStates();
            SetPlayerRoles();
        }

        private void ResetStates ()
        {
            amIJaguar = false;
            playerTurn = "none";
            selectedPieceCoordinates = new Vector2(-1, -1);
            selectedPositionCoordinates = new Vector2(-1, -1);
            winner = "none";
            dogCounter = 0;

            if (!playroomKit.IsHost()) return;

            playroomKit.SetState("winner", "none", true);
            playroomKit.SetState("dogCounter", 0, true);
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

        private void ChooseJaguarPlayer ()
        {
            if (!playroomKit.IsHost()) return;
            Debug.Log("Choosing Jaguar Player...");
            int randomJaguarSelection = Random.Range(0, 2);
            string player = randomJaguarSelection == 0 ? "player1" : "player2";
            playroomKit.SetState("jaguarPlayer", player, true);
            playroomKit.SetState("playerTurn", player, true);
        }

        private void OnJaguarPlayerChosen (string jaguarPlayer)
        {
            amIJaguar = (playerRole == jaguarPlayer);
            InterfaceManager.Instance.SetupCurrentPlayerInterface(amIJaguar, playroomKit.MyPlayer().GetProfile().name);
            InterfaceManager.Instance.SetupOpponentPlayerInterface(!amIJaguar, GetOtherPlayerName());
            InterfaceManager.Instance.FadeOut();
            LocalInputController.Instance.SetPlayerJaguar(amIJaguar);
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

        private void CheckWinnerUpdate (string winner)
        {
            if (this.winner == winner) return;
            Debug.Log("Winner updated: " + winner);

            this.winner = winner;
            bool isJaguarWinner = (winner == "jaguar");
            bool isPlayerWinner = (isJaguarWinner && amIJaguar) || (!isJaguarWinner && !amIJaguar);
            InterfaceManager.Instance.SetWinner(isPlayerWinner, isJaguarWinner);
        }

        private void CheckDogCounterUpdate (int dogCounter)
        {
            if (this.dogCounter == dogCounter) return;
            Debug.Log("DogCounter updated: " + dogCounter);

            this.dogCounter = dogCounter;
            InterfaceManager.Instance.SetDogCounter(dogCounter);
        }

        private void CheckPlayerTurnUpdate (string playerTurn)
        {
            if (this.playerTurn == playerTurn) return;
            Debug.Log("PlayerTurn updated: " + playerTurn);

            this.playerTurn = playerTurn;
            bool isCurrentPlayerTurn = (playerTurn == playerRole);
            InterfaceManager.Instance.SetPlayerTurnText(isCurrentPlayerTurn);
            LocalInputController.Instance.SetPlayerTurn(isCurrentPlayerTurn);
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
            selectedPiece.OnInteraction(false);
            BoardManager.Instance.ApplyMove(selectedPiece, selectedPosition);
            CheckWinConditions();
        }

        private void CheckWinConditions ()
        {
            if (!playroomKit.IsHost()) return;
            if (BoardManager.Instance.IsJaguarLocked()) playroomKit.SetState("winner", "dogs", true);
            if (dogCounter >= 6) playroomKit.SetState("winner", "jaguar", true);
        }

        public void OnPlayerFinishedTurn ()
        {
            playroomKit.SetState("playerTurn", (playerTurn == "player1") ? "player2" : "player1", true);
        }

        public void OnPlayerSelectedPiece (Vector2 selectedPieceCoordinates)
        {
            playroomKit.SetState("selectedPieceCoordinates", selectedPieceCoordinates, true);
        }

        public void OnPlayerSelectedPosition (Vector2 selectedPositionCoordinates, bool isJump)
        {
            playroomKit.SetState("selectedPositionCoordinates", selectedPositionCoordinates, true);
            if (isJump) OnJaguarJumped();
        }

        private void OnJaguarJumped()
        {
            int currentDogCount = playroomKit.GetState<int>("dogCount");
            currentDogCount++;
            playroomKit.SetState<int>("dogCount", currentDogCount, true);
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