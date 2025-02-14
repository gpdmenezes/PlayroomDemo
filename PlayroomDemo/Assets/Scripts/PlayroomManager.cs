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

        private PlayroomKit _playroomKit = new();
        private static readonly List<PlayroomKit.Player> currentPlayers = new();
        private bool hasMatchStarted = false;

        private void Start()
        {
            _playroomKit.InsertCoin(new InitOptions()
            {
                allowGamepads = false,
                skipLobby = false,
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() {
                    {"jaguarPlayer", "none"},
                    {"playerTurn", "none"},
            },
            }, () => {
                _playroomKit.OnPlayerJoin(AddPlayer);
            });
        }

        private void Update()
        {
            if (playerJoined)
            {
                var myPlayer = _playroomKit.MyPlayer();
                var index = currentPlayers.IndexOf(myPlayer);
            }

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
            if (!_playroomKit.IsHost()) return;
            SelectRandomJaguarPlayer();
        }

        private void SelectRandomJaguarPlayer()
        {
            int randomJaguarSelection = Random.Range(0, 2);
            if (randomJaguarSelection == 0)
            {
                _playroomKit.SetState("jaguarPlayer", "player1", true);
                InterfaceManager.Instance.SetJaguarPlayerName(currentPlayers[0].GetProfile().name);
                InterfaceManager.Instance.SetDogPlayerName(currentPlayers[1].GetProfile().name);
            }
            else
            {
                _playroomKit.SetState("jaguarPlayer", "player2", true);
                InterfaceManager.Instance.SetJaguarPlayerName(currentPlayers[1].GetProfile().name);
                InterfaceManager.Instance.SetDogPlayerName(currentPlayers[0].GetProfile().name);
            }
        }

        public static void AddPlayer(PlayroomKit.Player player)
        {
            currentPlayers.Add(player);
            Debug.Log(player.GetProfile().name + " joined the game! (PlayerID: " + player.id + ")");
            playerJoined = true;
            player.OnQuit(RemovePlayer);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void RemovePlayer(string playerID)
        {
            Debug.Log("PlayerID " + playerID + " left the game.");
        }
    }

}