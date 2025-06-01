namespace Platform {
    using UnityEngine;

    public class Platform {
        protected readonly GameObject PlatformObject;
        protected const float MarginToReposition = 2f; // To ensure that platforms disappear and appear outside player visibility

        public float PlatformY => PlatformObject.transform.position.y;
        
        public Platform(GameObject platform) {
            PlatformObject = platform;
        }
        
        public bool IsOutsidePlayerVisibility(float playerY, float playerToBottomScreenDist) {
            var distanceToPlatform = playerY - PlatformObject.transform.position.y;
            return distanceToPlatform >= playerToBottomScreenDist + MarginToReposition;
        }
        
        public void Move(Vector2 newPosition) {
            PlatformObject.transform.position = newPosition;
        }

        public void SetColliderActive(bool isEnables) {
            if (!PlatformObject.TryGetComponent<BoxCollider2D>(out var collider)) {
                return;
            }

            if (collider.enabled != isEnables) collider.enabled = isEnables;
        }

        public virtual void FrameUpdate() {}
    }
}