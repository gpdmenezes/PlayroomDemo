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
                SelectPiece();
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

        private void SelectPiece ()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Piece"))
                {
                    selectedPiece = hit.transform.GetComponent<BoardPiece>();
                    selectedPiece.OnPieceInteraction(true);
                }
            }
        }
    }
}