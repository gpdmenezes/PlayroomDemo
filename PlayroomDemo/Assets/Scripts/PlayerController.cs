using UnityEngine;

namespace PlayroomDemo
{
    public class PlayerController : MonoBehaviour
    {
        private BoardPiece selectedPiece = null;
        private bool canClick = false;

        private void Update ()
        {
            if (!canClick) return;

            if (Input.GetMouseButtonDown(1))
            {
                DeselectPiece();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                CheckMouseRaycast();
            }
        }

        private void DeselectPiece ()
        {
            if (selectedPiece)
            {
                selectedPiece.OnPieceInteraction(false);
                selectedPiece = null;
            }
        }

        private void CheckMouseRaycast ()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Piece"))
                {
                    OnPieceSelection(hit);
                    return;
                }

                if (hit.collider.CompareTag("Position"))
                {
                    OnPositionSelection(hit);
                }
            }
        }

        private void OnPieceSelection (RaycastHit hit)
        {
            if (selectedPiece != null) DeselectPiece();
            selectedPiece = hit.transform.GetComponent<BoardPiece>();
            selectedPiece.OnPieceInteraction(true);
        }

        private void OnPositionSelection (RaycastHit hit)
        {
            if (selectedPiece == null) return;
            BoardPosition boardPosition = hit.transform.GetComponent<BoardPosition>();
            if (boardPosition.IsOccupied()) return;

            if (selectedPiece.IsJaguar() && selectedPiece.IsBoardPositionValidForJump(boardPosition))
            {
                selectedPiece.RemoveJumpedPiece(boardPosition);
                ApplyPositionMove(boardPosition);
                return;
            }

            if (selectedPiece.IsBoardPositionValidForMove(boardPosition))
            {
                ApplyPositionMove(boardPosition);
                return;
            }
        }

        private void ApplyPositionMove (BoardPosition boardPosition)
        {
            BoardPiece oldPiece = selectedPiece;
            DeselectPiece();
            oldPiece.SetBoardPosition(boardPosition);
        }
    }
}