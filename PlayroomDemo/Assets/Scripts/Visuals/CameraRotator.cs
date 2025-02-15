using UnityEngine;

namespace PlayroomDemo.Visuals
{
    public class CameraRotator : MonoBehaviour
    {
        private float rotationSpeed = 50f;

        private void LateUpdate()
        {
            float rotationInput = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0, rotationInput, 0));
        }
    }
}