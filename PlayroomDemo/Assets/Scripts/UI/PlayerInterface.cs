using UnityEngine;
using UnityEngine.UI;

namespace PlayroomDemo.UI
{
    public class PlayerInterface : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image banner = null;
        [SerializeField] private Image bannerBottom = null;
        [SerializeField] private Image icon = null;
        [SerializeField] private Text playerName = null;
        [SerializeField] private Text playerTurn = null;

        [Header("Icons")]
        [SerializeField] private Sprite jaguarSprite = null;
        [SerializeField] private Sprite dogSprite = null;

        public void SetupInterface (bool isJaguar, string playerName)
        {
            this.playerName.text = playerName;
            if (isJaguar)
            {
                icon.sprite = jaguarSprite;
                icon.rectTransform.sizeDelta = new Vector2(170, 100);
                banner.color = Color.yellow;
                bannerBottom.color = Color.yellow;
            }
            else
            {
                icon.sprite = dogSprite;
                icon.rectTransform.sizeDelta = new Vector2(160, 110);
                banner.color = Color.green;
                bannerBottom.color = Color.green;
            }
        }

        public void SetPlayerTurnText (bool isMyTurn)
        {
            if (isMyTurn)
            {
                playerTurn.text = "PLAYING";
            }
            else
            {
                playerTurn.text = "";
            }
        }
    }
}
