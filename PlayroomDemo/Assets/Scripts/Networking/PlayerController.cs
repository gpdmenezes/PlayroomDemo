using PlayroomDemo.Board;
using UnityEngine;

namespace PlayroomDemo.Networking
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        [SerializeField] private Camera mainCamera = null;

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
            if (Input.GetMouseButtonDown(0))
            {
                CheckMouseClickRaycast();
            }
        }

        private void CheckMouseClickRaycast ()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Piece"))
                {
                    OnPieceClicked(hit);
                    return;
                }

                if (hit.collider.CompareTag("Position"))
                {
                    OnPositionClicked(hit);
                }
            }
        }

        private void OnPieceClicked (RaycastHit hit)
        {
            Debug.Log("Piece - Clicked.");
            selectedPiece = hit.transform.GetComponent<BoardPiece>();
            if ((!isPlayerJaguar && selectedPiece.IsJaguar()) || isPlayerJaguar && !selectedPiece.IsJaguar()) return;
            Debug.Log("Piece - Message Sent.");
            PlayroomManager.Instance.OnPlayerSelectedPiece(selectedPiece.GetBoardPosition().GetCoordinates());
        }

        private void OnPositionClicked (RaycastHit hit)
        {
            Debug.Log("Position - Clicked.");
            if (selectedPiece == null) return;
            BoardPosition boardPosition = hit.transform.GetComponent<BoardPosition>();
            if (boardPosition.IsOccupied()) return;

            if (!selectedPiece.IsJaguar())
            {
                Debug.Log("Position - Is Jaguar.");
                if (!selectedPiece.IsBoardPositionValidForMove(boardPosition)) return;
            }
            else
            {
                Debug.Log("Position - Not Jaguar.");
                if (!selectedPiece.IsBoardPositionValidForJump(boardPosition) && !selectedPiece.IsBoardPositionValidForMove(boardPosition)) return;
            }

            Debug.Log("Position - Message Sent.");
            PlayroomManager.Instance.OnPlayerSelectedPosition(boardPosition.GetCoordinates());
            isPlayerTurn = false;
            Invoke(nameof(FinishPlayerTurn), 0.5f);
        }

        private void FinishPlayerTurn()
        {
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

        public void SetReceivedMove (BoardPiece selectedPiece, BoardPosition boardPosition)
        {
            Debug.Log("MOVE - Piece: " + selectedPiece + " / Position: " + boardPosition);
            if (selectedPiece.IsJaguar() && selectedPiece.IsBoardPositionValidForJump(boardPosition))
            {
                selectedPiece.RemoveJumpedPiece(boardPosition);
                selectedPiece.SetBoardPosition(boardPosition);
                return;
            }

            if (selectedPiece.IsBoardPositionValidForMove(boardPosition))
            {
                selectedPiece.SetBoardPosition(boardPosition);
                return;
            }
        }
    }
}