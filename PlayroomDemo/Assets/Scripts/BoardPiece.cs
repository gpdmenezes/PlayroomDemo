using UnityEngine;

namespace PlayroomDemo
{
    public class BoardPiece : MonoBehaviour
    {
        [SerializeField] private bool isJaguar = false;
        [SerializeField] private GameObject selectionMarker = null;

        private bool isDead = false;
        private BoardPosition currentPosition = null;

        public void ResetPiece ()
        {
            isDead = false;
            currentPosition = null;
            selectionMarker.SetActive(false);
        }

        public void SetBoardPosition (BoardPosition boardPosition)
        {
            if (currentPosition != null) currentPosition.ResetPosition();
            this.currentPosition = boardPosition;
            currentPosition.SetOccupation(true);
            MoveToCurrentPosition();
        }

        private void MoveToCurrentPosition ()
        {
            transform.position = currentPosition.transform.position;
        }

        public void OnPieceInteraction (bool isSelection)
        {
            selectionMarker.SetActive(isSelection);
            currentPosition.ShouldMarkAvailableNeighbors(isSelection);
            if (isJaguar) currentPosition.ShouldMarkAvailableJumps(isSelection);
        }

        public bool IsBoardPositionValidForMove (BoardPosition boardPosition)
        {
            return currentPosition.IsNeighborPosition(boardPosition);
        }
    }
}