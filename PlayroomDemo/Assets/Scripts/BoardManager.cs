using UnityEngine;

namespace PlayroomDemo
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance;

        [SerializeField] private BoardPosition[] boardPositions;
        [SerializeField] private BoardPiece[] dogPieces;
        [SerializeField] private BoardPiece jaguarPiece;

        private const int dogPieceBoardStart = 1;

        private void Awake ()
        {
            Instance = this;
            //ResetBoard();
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

        public BoardPosition GetBoardPositionByCoordinate (Vector2 givenCoordinates)
        {
            foreach (BoardPosition boardPosition in boardPositions)
            {
                Vector2 coordinates = boardPosition.GetCoordinates();
                if (coordinates.x == givenCoordinates.x && coordinates.y == givenCoordinates.y) return boardPosition;
            }
            return null;
        }

        public BoardPiece GetBoardPieceByPosition (BoardPosition boardPosition)
        {
            foreach (BoardPiece boardPiece in dogPieces)
            {
                if (boardPiece.HasBeenJumped()) continue;
                if (boardPiece.GetBoardPosition() == boardPosition) return boardPiece;
            }
            return null;
        }
    }
}