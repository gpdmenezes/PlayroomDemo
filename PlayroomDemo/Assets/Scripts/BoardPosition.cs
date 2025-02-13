using UnityEngine;

namespace PlayroomDemo
{
    public class BoardPosition : MonoBehaviour
    {
        [SerializeField] private GameObject occupationMarker;
        [SerializeField] private BoardPosition[] neighborPositions;

        private bool isOccupied = false;

        public bool IsOccupied () { return isOccupied; }
        public void SetOccupation (bool isOccupied) { this.isOccupied = isOccupied; }

        public void ResetPosition ()
        {
            isOccupied = false;
            ShouldEnableOccupationMarker(false);
            ShouldMarkAvailableNeighbors(false);
        }

        public void ShouldMarkAvailableNeighbors (bool shouldMark)
        {
            foreach (BoardPosition boardPosition in neighborPositions)
            {
                if (boardPosition.IsOccupied()) continue;
                boardPosition.ShouldEnableOccupationMarker(shouldMark);
            }
        }

        public void ShouldEnableOccupationMarker (bool shouldEnable)
        {
            occupationMarker.SetActive(shouldEnable);
        }

        public bool IsNeighborPosition (BoardPosition boardPosition)
        {
            for (int i = 0; i < neighborPositions.Length; i++)
            {
                if (neighborPositions[i] == boardPosition) return true;
            }
            return false;
        }
    }
}