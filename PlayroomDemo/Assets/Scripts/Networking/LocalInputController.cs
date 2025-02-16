using PlayroomDemo.Board;
using UnityEngine;

namespace PlayroomDemo.Networking
{
    public class LocalInputController : MonoBehaviour
    {
        public static LocalInputController Instance;

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
            selectedPiece = hit.transform.GetComponent<BoardPiece>();
            if ((!isPlayerJaguar && selectedPiece.IsJaguar()) || isPlayerJaguar && !selectedPiece.IsJaguar()) return;
            PlayroomManager.Instance.OnPlayerSelectedPiece(selectedPiece.GetBoardPosition().GetCoordinates());
        }

        private void OnPositionClicked (RaycastHit hit)
        {
            if (selectedPiece == null) return;
            BoardPosition boardPosition = hit.transform.GetComponent<BoardPosition>();
            if (boardPosition.IsOccupied()) return;

            if (!selectedPiece.IsJaguar())
            {
                if (!selectedPiece.IsBoardPositionValidForMove(boardPosition)) return;
            }
            else
            {
                if (!selectedPiece.IsBoardPositionValidForJump(boardPosition) && !selectedPiece.IsBoardPositionValidForMove(boardPosition)) return;
            }

            PlayroomManager.Instance.OnPlayerSelectedPosition(boardPosition.GetCoordinates());
            isPlayerTurn = false;
            Invoke(nameof(FinishPlayerTurn), 1f);
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
    }
}