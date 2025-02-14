using UnityEngine;

namespace PlayroomDemo
{
    public class BoardPiece : MonoBehaviour
    {
        [SerializeField] private bool isJaguar = false;
        [SerializeField] private BoxCollider boxCollider = null;
        [SerializeField] private GameObject model = null;
        [SerializeField] private GameObject selectionMarker = null;

        private bool hasBeenJumped = false;
        private BoardPosition currentPosition = null;

        public bool IsJaguar () { return isJaguar; }
        public bool HasBeenJumped () { return hasBeenJumped; }
        public BoardPosition GetBoardPosition() { return currentPosition; }

        public void ResetPiece ()
        {
            hasBeenJumped = false;
            currentPosition = null;
            boxCollider.enabled = true;
            model.SetActive(true);
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

        public bool IsBoardPositionValidForJump (BoardPosition boardPosition)
        {
            return currentPosition.IsJumpPosition(boardPosition);
        }

        public void RemoveJumpedPiece (BoardPosition boardPosition)
        {
            BoardPiece jumpedPiece = currentPosition.GetJumpedPiece(boardPosition);
            jumpedPiece.OnPieceJumped();
        }

        public void OnPieceJumped ()
        {
            hasBeenJumped = true;
            boxCollider.enabled = false;
            model.SetActive(false);
            currentPosition.ResetPosition();
        }
    }
}