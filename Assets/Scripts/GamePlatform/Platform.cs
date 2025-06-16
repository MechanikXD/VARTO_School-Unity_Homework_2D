using UnityEngine;

namespace GamePlatform {
    public class Platform {
        private readonly GameObject _platformObject;
        private const float MarginToReposition = 2f; // To ensure that platforms disappear and appear outside player visibility

        public float PlatformY => _platformObject.transform.position.y;
        
        public Platform(GameObject platformReference) {
            _platformObject = Object.Instantiate(platformReference);
        }
        
        public bool IsOutsidePlayerVisibility(float playerY, float playerToBottomScreenDist) {
            var distanceToPlatform = playerY - _platformObject.transform.position.y;
            return distanceToPlatform >= playerToBottomScreenDist + MarginToReposition;
        }
        
        public void Move(Vector2 newPosition) {
            _platformObject.transform.position = newPosition;
        }

        public void SetColliderActive(bool isEnables) {
            if (!_platformObject.TryGetComponent<BoxCollider2D>(out var collider)) {
                return;
            }

            if (collider.enabled != isEnables) collider.enabled = isEnables;
        }
    }
}