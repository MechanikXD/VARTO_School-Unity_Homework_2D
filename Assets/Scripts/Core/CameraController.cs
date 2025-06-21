using UnityEngine;

namespace Core {
    public class CameraController : MonoBehaviour {
        [SerializeField] private Transform target;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float smoothness;
        [SerializeField] private float verticalOffset;

        private void Update() => MoveCameraAfterPlayer();

        private void MoveCameraAfterPlayer() {
            var cameraPosition = cameraTransform.position;
            var newPosition = new Vector3(cameraPosition.x, target.position.y + verticalOffset, cameraPosition.z);

            cameraTransform.position = Vector3.Lerp(cameraPosition, newPosition, smoothness * Time.deltaTime);
        }
    }
}