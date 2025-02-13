using UnityEngine;

namespace PlayroomDemo
{
    public class PlayerController : MonoBehaviour
    {
        private BoardPiece selectedPiece = null;

        private void Update ()
        {
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
                    if (selectedPiece != null) DeselectPiece();
                    selectedPiece = hit.transform.GetComponent<BoardPiece>();
                    selectedPiece.OnPieceInteraction(true);
                    return;
                }

                if (hit.collider.CompareTag("Position"))
                {
                    if (selectedPiece == null) return;
                    BoardPosition boardPosition = hit.transform.GetComponent<BoardPosition>();
                    if (boardPosition.IsOccupied() || !selectedPiece.IsBoardPositionValid(boardPosition)) return;
                    selectedPiece.SetBoardPosition(boardPosition);
                    DeselectPiece();
                }
            }
        }
    }
}