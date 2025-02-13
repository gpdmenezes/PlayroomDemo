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

        public void SetCurrentBoardPosition (BoardPosition boardPosition)
        {
            this.currentPosition = boardPosition;
        }

        public void OnPieceInteraction (bool isSelection)
        {
            selectionMarker.SetActive(isSelection);
            currentPosition.ShouldMarkAvailableNeighbors(isSelection);
        }
    }
}