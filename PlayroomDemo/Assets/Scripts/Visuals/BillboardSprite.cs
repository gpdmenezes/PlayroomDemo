using UnityEngine;

namespace PlayroomDemo.Visuals
{
    public class BillboardSprite : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private Renderer objectRenderer;

        private void Start()
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }

        private void LateUpdate()
        {
            Vector3 newRotation = mainCamera.transform.eulerAngles;
            newRotation.x = 0;
            newRotation.z = 0;
            transform.eulerAngles = newRotation;
        }
    }
}