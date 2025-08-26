using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using GamePlatform;
using Player;
using UI.Controllers;
using UI.Models;

namespace Core {
    /// Singleton class to manage current scene
    public class MySceneManager : MonoBehaviour {
        private static MySceneManager _instance;
        private Scene _currentScene;
        private Transform _playerTransform;
        private bool _gameIsActive;
        private DateTime _sessionStart;
        
        private List<Platform> _platformPool;  // Pool of platform to build level from. (Should be on scene)
        private GameObject _platformPrefab;
        private float _platformRepositionDistance;   // Distance from platform to player when can safety reposition
        private int _nextPlatformToMoveIndex;   // Iterate list instead of queue
        private const int PlatformCount = 5;
        
        private const float SceneCenter = 0.2f; // A bit offset, since it looks better
        private const float ScreenRelativePlatformDeviation = 0.6f; // part of the half of the screen from center
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
            _sessionStart = DateTime.Now;
            
            SetupPlatforms();
            SubscribeToInvents();
        }

        private void Update() => UpdatePlatforms();

        private void OnDestroy() {
            UnsubscribeFromEvents();
            StoreSessionData();
        }

        private void InstantiateSelf() {
            _currentScene = SceneManager.GetActiveScene();
            _platformPool = new List<Platform>(PlatformCount);
            _platformPrefab = Resources.Load<GameObject>("Prefabs/Platform");
            LoadPreviousGameData();
            
            SessionModel.CurrentHeight = 0;
            SessionModel.CurrentScore = 0;

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

            _gameIsActive = true;
        }

        private void SubscribeToInvents() {
            GameUIController.RestartButtonPressedEvent += RestartGameSession;
            GameUIController.ExitButtonPressedEvent += ExitGameSession;
            GameUIController.PauseButtonPressedEvent += PauseGameSession;
            GameUIController.ResumeButtonPressedEvent += ResumeGameSession;
        }

        private void UnsubscribeFromEvents() {
            GameUIController.RestartButtonPressedEvent -= RestartGameSession;
            GameUIController.ExitButtonPressedEvent -= ExitGameSession;
            GameUIController.PauseButtonPressedEvent -= PauseGameSession;
            GameUIController.ResumeButtonPressedEvent -= ResumeGameSession;
        }
        
        private void UpdatePlatforms() {
            if (_platformPool == null || !_gameIsActive) return;

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

        private void RestartGameSession() {
            _gameIsActive = false;
            SaveCurrentGameData();
            var currentSceneIndex = _currentScene.buildIndex;
            ResumeGameSession();
            SceneManager.LoadScene(currentSceneIndex);
        }
        
        private void ExitGameSession() {
            _gameIsActive = false;
            SaveCurrentGameData();
            SceneManager.LoadScene("MainMenuScene");
        }

        private static void PauseGameSession() => Time.timeScale = 0;

        private static void ResumeGameSession() => Time.timeScale = 1;

        // ReSharper disable Unity.PerformanceAnalysis
        public void EndCurrentSession() {
            _gameIsActive = false;
            SaveCurrentGameData();
            if (Camera.main != null) {
                Camera.main.GetComponent<CameraController>().enabled = false;
            }
            var player = _playerTransform.gameObject;
            player.GetComponent<PlayerController>().enabled = false;
            Destroy(player, 1);
            
            GameUIController.OnGameOver();
        }

        private static void LoadPreviousGameData() {
            // TODO: Move PlayerPrefs fields into separate enum 
            if (PlayerPrefs.HasKey("Best Height")) {
                SessionModel.BestHeight = PlayerPrefs.GetInt("Best Height");
            }
            if (PlayerPrefs.HasKey("Best Score")) {
                SessionModel.BestScore = PlayerPrefs.GetInt("Best Score");
            }
        }

        private void SaveCurrentGameData() {
            if (SessionModel.BestHeight < SessionModel.CurrentHeight) {
                PlayerPrefs.SetInt("Best Height", SessionModel.CurrentHeight);
                SessionModel.BestHeight = SessionModel.CurrentHeight;
            }
            if (SessionModel.BestScore < SessionModel.CurrentScore) {
                PlayerPrefs.SetInt("Best Score", SessionModel.CurrentScore);
                SessionModel.BestScore = SessionModel.CurrentScore;
            }
        }

        private void StoreSessionData() {
            var previousSessions = PlayerPrefs.GetString("Sessions");
            previousSessions += $"{_sessionStart:u}: Height - {SessionModel.CurrentHeight}; Score - {SessionModel.CurrentScore}\n";
            PlayerPrefs.SetString( "Sessions", previousSessions);
        }
    }
}