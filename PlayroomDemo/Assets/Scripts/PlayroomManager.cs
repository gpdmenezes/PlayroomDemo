using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;

public class PlayroomManager : MonoBehaviour
{
    private PlayroomKit _playroomKit = new();
    
    private void Start()
    {
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() {
            {"score", 0},
        },
        }, () => {
            // Game launch logic here
        });
    }
}
