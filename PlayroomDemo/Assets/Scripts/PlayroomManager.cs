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
        [SerializeField] private static bool playerJoined;

        private PlayroomKit playroomKit = new();
        private static readonly List<Player> currentPlayers = new();
        private bool hasMatchStarted = false;

        private void Start()
        {
            playroomKit.InsertCoin(new InitOptions()
            {
                allowGamepads = false,
                skipLobby = false,
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() {
                    {"jaguarPlayer", "none"},
                    {"playerTurn", "none"},
            },
            }, () => {
                playroomKit.OnPlayerJoin(AddPlayer);
            });

            playroomKit.WaitForState("jaguarPlayer", (value) => 
            {
                OnJaguarPlayerUpdate(value);
            });

            playroomKit.WaitForState("playerTurn", (value) =>
            {
                OnPlayerTurnUpdate(value);
            });
        }

        private void Update ()
        {
            if (!playerJoined) return;

            Player myPlayer = playroomKit.MyPlayer();
            int index = currentPlayers.IndexOf(myPlayer);

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
            if (!playroomKit.IsHost()) return;
            SelectRandomJaguarPlayer();
        }

        private void SelectRandomJaguarPlayer()
        {
            int randomJaguarSelection = Random.Range(0, 2);
            if (randomJaguarSelection == 0)
            {
                playroomKit.SetState("jaguarPlayer", "player1", true);
                playroomKit.SetState("playerTurn", "player1", true);
            }
            else
            {
                playroomKit.SetState("jaguarPlayer", "player2", true);
                playroomKit.SetState("playerTurn", "player2", true);
            }
        }

        public void OnJaguarPlayerUpdate (string jaguarPlayer)
        {
            Debug.Log("OnJaguarPlayerUpdate");
            bool isPlayer1Jaguar = (jaguarPlayer == "player1");
            InterfaceManager.Instance.SetupPlayerInterface(true, currentPlayers[0].GetProfile().name, isPlayer1Jaguar);
            InterfaceManager.Instance.SetupPlayerInterface(false, currentPlayers[1].GetProfile().name, !isPlayer1Jaguar);
        }

        public void OnPlayerTurnUpdate (string playerTurn)
        {
            Debug.Log("OnPlayerTurnUpdate");
            bool isPlayer1Turn = (playerTurn == "player1");
            InterfaceManager.Instance.SetPlayerTurnText(isPlayer1Turn);
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