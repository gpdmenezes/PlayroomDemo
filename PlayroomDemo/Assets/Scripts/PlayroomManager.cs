using System.Collections.Generic;
using UnityEngine;
using Playroom;
using AOT;
using System;
using Random = UnityEngine.Random;

namespace PlayroomDemo
{
    public class PlayroomManager : MonoBehaviour
    {
        [SerializeField] private static bool playerJoined;

        private PlayroomKit _playroomKit = new();
        private static readonly List<PlayroomKit.Player> players = new();
        private static readonly List<GameObject> playerGameObjects = new();
        private static Dictionary<string, GameObject> PlayerDict = new();

        private void Start()
        {
            _playroomKit.InsertCoin(new InitOptions()
            {
                allowGamepads = false,
                skipLobby = false,
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() {
                    {"score", 0},
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
                var index = players.IndexOf(myPlayer);

                playerGameObjects[index].GetComponent<PlayerController>().Move();
                playerGameObjects[index].GetComponent<PlayerController>().Jump();

                players[index].SetState("posX", playerGameObjects[index].GetComponent<Transform>().position.x);
                players[index].SetState("posY", playerGameObjects[index].GetComponent<Transform>().position.y);
            }

            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    var posX = players[i].GetState<float>("posX");
                    var posY = players[i].GetState<float>("posY");
                    Vector3 newPos = new Vector3(posX, posY, 0);

                    if (playerGameObjects != null)
                        playerGameObjects[i].GetComponent<Transform>().position = newPos;
                }
            }
        }

        public static void AddPlayer(PlayroomKit.Player player)
        {
            GameObject playerObj = (GameObject)Instantiate(Resources.Load("Player"), new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0), Quaternion.identity);

            playerObj.GetComponent<SpriteRenderer>().color = player.GetProfile().color;
            Debug.Log(player.GetProfile().name + " Joined the game!" + "id: " + player.id);

            PlayerDict.Add(player.id, playerObj);
            players.Add(player);
            playerGameObjects.Add(playerObj);

            playerJoined = true;

            player.OnQuit(RemovePlayer);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void RemovePlayer(string playerID)
        {
            if (PlayerDict.TryGetValue(playerID, out GameObject player))
            {
                Destroy(player);
            }
            else
            {
                Debug.LogWarning("Player not in dictionary!");
            }

        }
    }

}