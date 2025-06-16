using UnityEngine;

namespace GamePlatform {
    public class Platform {
        private GameObject _platformObject;
        private const float MarginToReposition = 3f; // To ensure that platforms disappear and appear outside player visibility

        public float PlatformY => _platformObject.transform.position.y;
        
        // TODO: Incorrect distance calculation
        public bool IsOutsidePlayerVisibility(float playerY, float playerToBottomScreenDist) {
            var distanceToPlatform = playerY - _platformObject.transform.position.y;
            return distanceToPlatform >= playerToBottomScreenDist + MarginToReposition;
        }
        
        public void Move(Vector2 newPosition) {
            _platformObject.transform.position = newPosition;
        }

        public void SetColliderActive(bool isEnables) {
            if (!_platformObject.TryGetComponent<BoxCollider2D>(out var platformCollider)) {
                return;
            }

            if (platformCollider.enabled != isEnables) platformCollider.enabled = isEnables;
        }
        
        public void InstantiateSelf(GameObject prefab) {
            _platformObject = Object.Instantiate(prefab);
        }

        public void DestroySelf() {
            Object.Destroy(_platformObject);
        }
    }
}