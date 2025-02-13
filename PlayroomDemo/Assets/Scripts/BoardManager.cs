using UnityEngine;

namespace PlayroomDemo
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private BoardPosition[] positions;
        [SerializeField] private PieceController[] dogPieces;
        [SerializeField] private PieceController jaguarPiece;
    }
}