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
                boardPosition.ResetPosition();
            }
        }

        private void ResetBoardPieces ()
        {
            jaguarPiece.ResetPiece();
            jaguarPiece.SetBoardPosition(boardPositions[0]);

            int dogPieceCounter = dogPieceBoardStart;
            foreach (BoardPiece dogPiece in dogPieces)
            {
                dogPiece.ResetPiece();
                dogPiece.SetBoardPosition(boardPositions[dogPieceCounter]);
                dogPieceCounter++;
            }
        }
    }
}