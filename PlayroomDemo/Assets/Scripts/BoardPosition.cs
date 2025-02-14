using System.Collections.Generic;
using UnityEngine;

namespace PlayroomDemo
{
    public class BoardPosition : MonoBehaviour
    {
        [SerializeField] private GameObject occupationMarker;
        [SerializeField] private int xPosition = 0;
        [SerializeField] private int yPosition = 0;
        [SerializeField] private BoardPosition[] neighborPositions;

        private bool isOccupied = false;

        public bool IsOccupied () { return isOccupied; }
        public void SetOccupation (bool isOccupied) { this.isOccupied = isOccupied; }
        public Vector2 GetCoordinates () { return new Vector2(xPosition, yPosition); }

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

        private void ShouldEnableOccupationMarker (bool shouldEnable)
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

        public void ShouldMarkAvailableJumps (bool shouldMark)
        {
            List<BoardPosition> availableJumps = ListAllAvailableJumps();
            foreach (BoardPosition boardPosition in availableJumps)
            {
                boardPosition.ShouldEnableOccupationMarker(shouldMark);
            }
        }

        public bool IsJumpPosition (BoardPosition boardPosition)
        {
            List<BoardPosition> availableJumps = ListAllAvailableJumps();
            if (availableJumps.Contains(boardPosition)) return true;
            return false;
        }

        private List<BoardPosition> ListAllAvailableJumps ()
        {
            List<BoardPosition> availableJumps = new List<BoardPosition>();
            foreach (BoardPosition boardPosition in neighborPositions)
            {
                if (!boardPosition.IsOccupied()) continue;
                Vector2 coordinatesDifference = (boardPosition.GetCoordinates() - GetCoordinates()) * 2;
                Vector2 coordinatesToCheck = GetCoordinates() + coordinatesDifference;
                BoardPosition positionToCheck = BoardManager.Instance.GetBoardPositionByCoordinate(coordinatesToCheck);
                if (positionToCheck != null && !positionToCheck.IsOccupied()) availableJumps.Add(positionToCheck);
            }
            return availableJumps;
        }

        public BoardPiece GetJumpedPiece (BoardPosition givenPosition)
        {
            Vector2 givenCoordinates = givenPosition.GetCoordinates();
            Vector2 coordinatesToCheck = (givenCoordinates - GetCoordinates()) / 2;
            Vector2 finalCoordinates = GetCoordinates() + coordinatesToCheck;
            BoardPosition position = BoardManager.Instance.GetBoardPositionByCoordinate(finalCoordinates);
            return BoardManager.Instance.GetBoardPieceByPosition(position);
        }
    }
}