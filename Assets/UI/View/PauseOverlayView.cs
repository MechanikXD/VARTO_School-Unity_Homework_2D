using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View {
    public class PauseOverlayView : MonoBehaviour {
        private Canvas _thisCanvas;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        public void OnEnable() => SubscribeToEvents();
        public void Start() {
            TryGetComponent(out _thisCanvas);                    // Cash attached canvas
            UIController.PauseButtonPressedEvent += ShowOverlay; // Event not directly bound to this canvas
            HideOverlay();                                       // Hide on scene load
        }

        public void OnDisable() => UnsubscribeFromEvents();

        public void OnDestroy() {
            UIController.PauseButtonPressedEvent -= ShowOverlay;
        }
        
        private void SubscribeToEvents() {
            UIController.ResumeButtonPressedEvent += HideOverlay;
            UIController.RestartButtonPressedEvent += MySceneManager.Instance.RestartGameSession;
            UIController.ExitButtonPressedEvent += MySceneManager.Instance.ExitGameSession;
            
            resumeButton.onClick.AddListener(UIController.OnResumeButtonPressed);
            restartButton.onClick.AddListener(UIController.OnRestartButtonPressed);
            exitButton.onClick.AddListener(UIController.OnExitButtonPressed);
        }

        private void UnsubscribeFromEvents() {
            UIController.ResumeButtonPressedEvent -= HideOverlay;
            UIController.RestartButtonPressedEvent -= MySceneManager.Instance.RestartGameSession;
            UIController.ExitButtonPressedEvent -= MySceneManager.Instance.ExitGameSession;
            
            resumeButton.onClick.RemoveListener(UIController.OnResumeButtonPressed);
            restartButton.onClick.RemoveListener(UIController.OnRestartButtonPressed);
            exitButton.onClick.RemoveListener(UIController.OnExitButtonPressed);
        }

        private void ShowOverlay() => _thisCanvas.enabled = true;

        private void HideOverlay() => _thisCanvas.enabled = false;
    }
}