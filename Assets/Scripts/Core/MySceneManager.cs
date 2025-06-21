using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using GamePlatform;
using UI.Models;

namespace Core {
    /// Singleton class to manage current scene
    public class MySceneManager : MonoBehaviour {
        private static MySceneManager _instance;
        private Scene _currentScene;
        private List<Platform> _platformPool;  // Pool of platform to build level from. (Should be on scene)
        private Transform _playerTransform;
        private float _platformRepositionDistance;   // Distance from platform to player when can safety reposition
        private GameObject _platformPrefab;

        private int _nextPlatformToMoveIndex;   // Iterate list instead of queue
        private const float SceneCenter = 0.2f; // A bit offset, since it looks better
        private const float ScreenRelativePlatformDeviation = 0.6f; // part of the half of the screen from center
        private const int PlatformCount = 5;
        private float _platformMaxHorizontalStep; 
        private float _platformVerticalStep;

        public static MySceneManager Instance {
            get {
                if (_instance != null) {
                    return _instance;
                }

                var obj = new GameObject("MySceneManager");
                _instance = obj.AddComponent<MySceneManager>();

                return _instance;
            }
        }

        public void Awake() => InstantiateSelf();

        public void Start() {
            // Separate from InstantiateSelf() initialization related to player.
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (Camera.main != null) 
                _platformRepositionDistance = _playerTransform.position.y - Camera.main.transform.position.y;
            
            SessionModel.CurrentHeight = 0;
            SessionModel.CurrentScore = 0;
            
            SetupPlatforms();
        }

        private void Update() => UpdatePlatforms();

        private void InstantiateSelf() {
            _currentScene = SceneManager.GetActiveScene();
            _platformPool = new List<Platform>(PlatformCount);
            _platformPrefab = Resources.Load<GameObject>("Prefabs/Platform");
            LoadPreviousGameData();

            if (Camera.main != null) {
                var mainCamera = Camera.main;
                var cameraHeight = mainCamera.orthographicSize * 2;
                var cameraWidth = cameraHeight * mainCamera.aspect;
                    
                _platformMaxHorizontalStep = cameraWidth / 2 * ScreenRelativePlatformDeviation;
                _platformVerticalStep = cameraHeight / PlatformCount;
            }
            
            for (var _ = 0; _ < PlatformCount; _++) {
                var newPlatform = new Platform();
                newPlatform.InstantiateSelf(_platformPrefab);
                _platformPool.Add(newPlatform);
            }
        }
        
        private void UpdatePlatforms() {
            if (_platformPool == null) return;

            for (var i = 0; i < PlatformCount; i++) {
                // Player above platform -> activate platform collider
                _platformPool[i].SetColliderActive(_platformPool[i].PlatformY <
                                                   _playerTransform.position.y - 0.5f);

                if (_platformPool[i].IsOutsidePlayerVisibility(_playerTransform.position.y,
                        _platformRepositionDistance)) {
                    RepositionPlatformOnIndex(i);
                }
            }
        }

        private void SetupPlatforms() {
            if (_platformPool == null) return;
            _platformPool[0].Move(Vector2.zero);
            AdvanceIndexInPool();
            
            for (var i = 1; i < PlatformCount; i++) {
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
            _nextPlatformToMoveIndex = (_nextPlatformToMoveIndex + 1) % PlatformCount;
        }

        private int GetHighestPlatformIndex() {
            return _nextPlatformToMoveIndex - 1 < 0 ?
                PlatformCount - 1 : _nextPlatformToMoveIndex - 1;
        }

        private void RepositionPlatformOnIndex(int index) {
            _platformPool[index].DestroySelf();
            _platformPool[index].InstantiateSelf(_platformPrefab);
            _platformPool[index].Move(GetCoordinatesForNextPlatform());
            _platformPool[index].SetColliderActive(false);
            AdvanceIndexInPool();
        }

        public void RestartGameSession() {
            SaveCurrentGameData();
            var currentSceneIndex = _currentScene.buildIndex;
            foreach (var platform in _platformPool) {
                platform.DestroySelf();
            }
            SceneManager.LoadScene(currentSceneIndex);
            ResumeGameSession();
        }
        
        public void ExitGameSession() {
            SaveCurrentGameData();
            Debug.LogError("Not implemented method: MySceneManager - ExitGameSession() ");
        }

        public void PauseGameSession() => Time.timeScale = 0;

        public void ResumeGameSession() => Time.timeScale = 1;

        private void LoadPreviousGameData() {
            // TODO: Move PlayerPrefs fields into separate enum 
            if (PlayerPrefs.HasKey("Best Height")) {
                SessionModel.BestHeight = PlayerPrefs.GetInt("Best Height");
            }
            if (PlayerPrefs.HasKey("Best Score")) {
                SessionModel.BestHeight = PlayerPrefs.GetInt("Best Score");
            }
        }

        private void SaveCurrentGameData() {
            if (PlayerPrefs.HasKey("Best Height") && SessionModel.BestHeight < SessionModel.CurrentHeight) {
                PlayerPrefs.SetInt("Best Height", SessionModel.CurrentHeight);
            }
            if (PlayerPrefs.HasKey("Best Score") && SessionModel.BestScore < SessionModel.CurrentScore) {
                PlayerPrefs.SetInt("Best Score", SessionModel.CurrentScore);
            }
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