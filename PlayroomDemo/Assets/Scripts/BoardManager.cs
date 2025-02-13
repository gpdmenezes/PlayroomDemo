using UnityEngine;

namespace PlayroomDemo
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private BoardPosition[] boardPositions;
        [SerializeField] private BoardPiece[] dogPieces;
        [SerializeField] private BoardPiece jaguarPiece;

        private const int dogPieceBoardStart = 1;

        private void Awake ()
        {
            ResetBoard();
        }

        public void ResetBoard ()
        {
            ResetBoardPositions();
            ResetBoardPieces();
        }

        private void ResetBoardPositions ()
        {
            foreach (BoardPosition boardPosition in boardPositions)
            {
                boardPosition.SetOccupation(false);
                boardPosition.ShouldEnableOccupationMarker(false);
            }
        }

        private void ResetBoardPieces ()
        {
            jaguarPiece.SetCurrentBoardPosition(boardPositions[0]);
            jaguarPiece.transform.position = boardPositions[0].transform.position;
            boardPositions[0].SetOccupation(true);

            int dogPieceCounter = dogPieceBoardStart;
            foreach (BoardPiece dogPiece in dogPieces)
            {
                dogPiece.SetCurrentBoardPosition(boardPositions[dogPieceCounter]);
                dogPiece.transform.position = boardPositions[dogPieceCounter].transform.position;
                boardPositions[dogPieceCounter].SetOccupation(true);
                dogPieceCounter++;
            }
        }
    }
}