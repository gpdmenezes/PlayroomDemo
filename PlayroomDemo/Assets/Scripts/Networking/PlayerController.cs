using PlayroomDemo.Board;
using UnityEngine;

namespace PlayroomDemo.Networking
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        private BoardPiece selectedPiece = null;
        private bool isPlayerJaguar = false;
        private bool isPlayerTurn = false;

        private void Awake ()
        {
            Instance = this;
        }

        private void Update ()
        {
            if (!isPlayerTurn) return;

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
            if ((!isPlayerJaguar && selectedPiece.IsJaguar()) || isPlayerJaguar && !selectedPiece.IsJaguar()) return;
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
            isPlayerTurn = false;
            PlayroomManager.Instance.OnPlayerFinishedTurn();
        }

        public void SetPlayerJaguar (bool isPlayerJaguar)
        {
            this.isPlayerJaguar = isPlayerJaguar;
        }

        public void SetPlayerTurn (bool isPlayerTurn)
        {
            this.isPlayerTurn = isPlayerTurn;
        }
    }
}