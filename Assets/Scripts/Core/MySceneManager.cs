using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using GamePlatform;

namespace Core {
    /// Singleton class to manage current scene
    public class MySceneManager : MonoBehaviour {
        private static MySceneManager _instance;
        private static Scene _currentScene;
        private static List<Platform> _platformPool;  // Pool of platform to build level from. (Should be on scene)
        private static Transform _playerPosition;
        private static float _platformRepositionDistance;   // Distance from platform to player when can safety reposition

        private int _nextPlatformToMoveIndex;   // Iterate list instead of queue
        private const float SceneCenter = 0.2f; // A bit offset, since it looks better
        private const float ScreenRelativePlatformDeviation = 0.6f; // part of the half of the screen from center
        private const int PlatformCount = 5;
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
                
                _playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
                _currentScene = SceneManager.GetActiveScene();

                _platformPool = new List<Platform>(PlatformCount);
                var groundObjects = GameObject.FindGameObjectWithTag("Ground");
                for (var _ = 0; _ < PlatformCount; _++) {
                    _platformPool.Add(new Platform(groundObjects));
                }
                // Alternatively: Keep first object in list and instantiate one less platform
                Destroy(groundObjects); // Destroy original to avoid duplicates.

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
                // Player above platform -> activate platform collider
                if (_platformPool[i].PlatformY < _playerPosition.position.y) {
                    _platformPool[i].SetColliderActive(true);
                }

                if (_platformPool[i].IsOutsidePlayerVisibility(_playerPosition.position.y,
                        _platformRepositionDistance)) {
                    RepositionPlatformOnIndex(i);
                }
            }
        }

        public void SetupScene(Vector2 startPlatformCoordinates) {
            if (_platformPool == null) return;
            _platformPool[0].Move(startPlatformCoordinates);
            AdvanceIndexInPool();
            
            for (var i = 1; i < _platformPool.Count; i++) {
                RepositionPlatformOnIndex(i);
            }
        }
        
        private Vector2 GetCoordinatesForNextPlatform() {
            var newX = SceneCenter +
                       Random.Range(-_platformMaxHorizontalStep, _platformMaxHorizontalStep);
            var newY = _platformPool[GetHighestPlatformIndex()].PlatformY + _platformVerticalStep;
            return new Vector2(newX, newY);
        }

        private void AdvanceIndexInPool() {
            _nextPlatformToMoveIndex = (_nextPlatformToMoveIndex + 1) % _platformPool.Count;
        }

        private int GetHighestPlatformIndex() {
            return _nextPlatformToMoveIndex - 1 < 0 ?
                _platformPool.Count -1 : _nextPlatformToMoveIndex - 1;
        }

        private void RepositionPlatformOnIndex(int index) {
            _platformPool[index].Move(GetCoordinatesForNextPlatform());
            _platformPool[index].SetColliderActive(false);
            AdvanceIndexInPool();
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