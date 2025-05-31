using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Core {
    /// Singleton class to manage current scene
    public class MySceneManager : MonoBehaviour {
        private static MySceneManager _instance;
        private static Scene _currentScene;
        private static List<GameObject> _platformPool;  // Pool of platform to build level from. (Should be on scene)
        private static Transform _playerPosition;
        private static float _platformRepositionDistance;   // Distance from platform to player when can safety reposition

        private int _nextPlatformToMoveIndex;   // Iterate list instead of queue
        private const float SceneCenter = 0.2f; // A bit offset, since it looks better
        private const float ScreenRelativePlatformDeviation = 0.6f; // part of the half of the screen from center
        private static float _platformMaxHorizontalStep; 
        private static float _platformVerticalStep;

        public static MySceneManager Instance {
            get {
                if (_instance != null) {
                    return _instance;
                }

                var obj = new GameObject("MySceneManager");
                _instance = obj.AddComponent<MySceneManager>();
                DontDestroyOnLoad(obj);
                
                _platformPool = GameObject.FindGameObjectsWithTag("Ground").ToList();
                _playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
                _currentScene = SceneManager.GetActiveScene();

                if (Camera.main != null) {
                    var mainCamera = Camera.main;
                    var cameraHeight = mainCamera.orthographicSize * 2;
                    var cameraWidth = cameraHeight * mainCamera.aspect;
                    
                    _platformMaxHorizontalStep = cameraWidth / 2 * ScreenRelativePlatformDeviation;
                    _platformVerticalStep = cameraHeight / _platformPool.Count;

                    _platformRepositionDistance =
                        _playerPosition.position.y - mainCamera.transform.position.y;
                }

                return _instance;
            }
        }

        private void Update() {
            for (var i = 0; i < _platformPool.Count; i++) {
                if (_platformPool[i].transform.position.y < _playerPosition.position.y) {
                    if (_platformPool[i].TryGetComponent<BoxCollider2D>(out var platformCollider)) {
                        if (!platformCollider.enabled) {
                            platformCollider.enabled = true;
                        }
                    }
                }

                if (PlatformOutsidePLayerVisibility(i)) {
                    MovePlatform(i, GetCoordinatesForNextPlatform());
                }
            }
        }

        public void SetupScene(Vector2 startPlatformCoordinates) {
            if (_platformPool == null) return;
            _platformPool[0].transform.position = startPlatformCoordinates;
            AdvanceIndexInPool();
            
            for (var i = 1; i < _platformPool.Count; i++) {
                MovePlatform(_nextPlatformToMoveIndex, GetCoordinatesForNextPlatform());
            }
        }
        // TODO: Move into separate Platform class
        private void MovePlatform(int platformIndex, Vector2 newPosition) {
            _platformPool[platformIndex].transform.position = newPosition;
            
            if (_platformPool[platformIndex].TryGetComponent<BoxCollider2D>(out var platformCollider)) {
                platformCollider.enabled = false;
            }
            
            AdvanceIndexInPool();
        }
        
        private Vector2 GetCoordinatesForNextPlatform() {
            var newX = SceneCenter +
                       Random.Range(-_platformMaxHorizontalStep, _platformMaxHorizontalStep);
            var newY = _platformPool[GetHighestPlatformIndex()].transform.position.y +
                       _platformVerticalStep;
            return new Vector2(newX, newY);
        }

        private void AdvanceIndexInPool() {
            _nextPlatformToMoveIndex = (_nextPlatformToMoveIndex + 1) % _platformPool.Count;
        }

        private int GetHighestPlatformIndex() {
            return _nextPlatformToMoveIndex - 1 < 0 ?
                _platformPool.Count -1 : _nextPlatformToMoveIndex - 1;
        }
        // TODO: Move into separate Platform class
        private bool PlatformOutsidePLayerVisibility(int platformIndex) {
            var distanceToPlatform = _playerPosition.position.y -
                                     _platformPool[platformIndex].transform.position.y;
            return distanceToPlatform >= _platformRepositionDistance + 2f;
        }

        public void ChangeAllColorsInScene() {
            foreach (var gameObj in _currentScene.GetRootGameObjects()) {
                if (gameObj.TryGetComponent<SpriteRenderer>(out var sprite)) {
                    sprite.color = Random.ColorHSV();
                }
            }
        }
    }
}