using DG.Tweening;
using UnityEngine;

namespace PlayroomDemo.Board
{
    public class BoardPiece : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private bool isJaguar = false;
        [SerializeField] private BoxCollider boxCollider = null;
        [SerializeField] private GameObject model = null;
        [SerializeField] private GameObject selectionMarker = null;

        [Header("Sounds")]
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private AudioClip[] clickSounds = null;
        [SerializeField] private AudioClip[] disableSounds = null;

        private bool hasBeenJumped = false;
        private BoardPosition currentPosition = null;

        public bool IsJaguar () { return isJaguar; }
        public bool HasBeenJumped () { return hasBeenJumped; }
        public BoardPosition GetBoardPosition() { return currentPosition; }

        public void ResetPiece ()
        {
            hasBeenJumped = false;
            currentPosition = null;
            boxCollider.enabled = true;
            model.SetActive(true);
            selectionMarker.SetActive(false);
        }

        public void SetBoardPosition (BoardPosition boardPosition)
        {
            if (currentPosition != null) currentPosition.ResetPosition();
            this.currentPosition = boardPosition;
            currentPosition.SetOccupation(true);
            MoveToCurrentPosition();
        }

        private void MoveToCurrentPosition ()
        {
            transform.DOMove(currentPosition.transform.position, 0.5f).SetEase(Ease.Linear);
        }

        public void OnInteraction (bool isSelection)
        {
            selectionMarker.SetActive(isSelection);
            currentPosition.ShouldMarkAvailableNeighbors(isSelection);
            if (isJaguar) currentPosition.ShouldMarkAvailableJumps(isSelection);
            if (isSelection) PlayInteractionSound();
        }

        private void PlayInteractionSound ()
        {
            int randomSoundIndex = Random.Range(0, 2);
            AudioClip randomSound = randomSoundIndex == 0 ? clickSounds[0] : clickSounds[1];
            audioSource.clip = randomSound;
            audioSource.Play();
        }

        public bool IsBoardPositionValidForMove (BoardPosition boardPosition)
        {
            return currentPosition.IsPositionNeighbor(boardPosition);
        }

        public bool IsBoardPositionValidForJump (BoardPosition boardPosition)
        {
            return currentPosition.IsPositionJumpable(boardPosition);
        }

        public void RemoveJumpedPiece (BoardPosition boardPosition)
        {
            BoardPiece jumpedPiece = currentPosition.GetJumpedPiece(boardPosition);
            jumpedPiece.OnPieceJumped();
        }

        public void OnPieceJumped ()
        {
            hasBeenJumped = true;
            transform.DOMoveY(-1f, 0.75f);
            PlayDisableSound();
            Invoke(nameof(DisablePiece), 1f);
        }

        private void PlayDisableSound ()
        {
            int randomSoundIndex = Random.Range(0, 2);
            AudioClip randomSound = randomSoundIndex == 0 ? disableSounds[0] : disableSounds[1];
            audioSource.clip = randomSound;
            audioSource.Play();
        }

        private void DisablePiece ()
        {
            boxCollider.enabled = false;
            model.SetActive(false);
            currentPosition.ResetPosition();
        }
    }
}