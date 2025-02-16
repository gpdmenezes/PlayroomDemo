using UnityEngine;
using UnityEngine.UI;

namespace PlayroomDemo.UI
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private Text winPlayerText = null;
        [SerializeField] private Text winJaguarText = null;
        [SerializeField] private Image banner = null;
        [SerializeField] private Image bannerBackground = null;
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private AudioClip winSound = null;
        [SerializeField] private AudioClip loseSound = null;

        public void SetupScreen (bool isPlayer, bool isJaguar)
        {
            winPlayerText.text = (isPlayer ? "You, The" : "Opponent, The");
            winJaguarText.text = (isJaguar ? "Jaguar" : "Dogs");
            banner.color = ((isJaguar ? Color.yellow : Color.green));
            bannerBackground.color = ((isJaguar ? Color.yellow : Color.green));
            audioSource.clip = (isPlayer ? winSound : loseSound);
            audioSource.Play();
        }
    }
}