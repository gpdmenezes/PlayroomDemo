using UnityEngine;

namespace PlayroomDemo
{
    public class BoardPosition : MonoBehaviour
    {
        [SerializeField] private BoardPosition[] neighborPositions;

        private bool isOccupied = false;
    }
}