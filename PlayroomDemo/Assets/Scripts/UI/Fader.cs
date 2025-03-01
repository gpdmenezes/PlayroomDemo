using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PlayroomDemo.UI
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private Image blackBackground;

        public void ResetFader ()
        {
            blackBackground.color = Color.black;
        }

        public void FadeOut ()
        {
            blackBackground.DOFade(0, 1.5f);
        }
    }
}