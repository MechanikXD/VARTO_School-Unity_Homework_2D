using UI.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.GameView {
    public class PauseOverlayView : MonoBehaviour {
        private Canvas _thisCanvas;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        public void OnEnable() => SubscribeToEvents();
        public void Start() {
            TryGetComponent(out _thisCanvas);                    // Cash attached canvas
            GameUIController.PauseButtonPressedEvent += ShowOverlay; // Event not directly bound to this canvas
            HideOverlay();                                       // Hide on scene load
        }

        public void OnDisable() => UnsubscribeFromEvents();

        public void OnDestroy() {
            GameUIController.PauseButtonPressedEvent -= ShowOverlay;
        }
        
        private void SubscribeToEvents() {
            GameUIController.ResumeButtonPressedEvent += HideOverlay;
            
            resumeButton.onClick.AddListener(GameUIController.OnResumeButtonPressed);
            restartButton.onClick.AddListener(GameUIController.OnRestartButtonPressed);
            exitButton.onClick.AddListener(GameUIController.OnExitButtonPressed);
        }

        private void UnsubscribeFromEvents() {
            GameUIController.ResumeButtonPressedEvent -= HideOverlay;
            
            resumeButton.onClick.RemoveListener(GameUIController.OnResumeButtonPressed);
            restartButton.onClick.RemoveListener(GameUIController.OnRestartButtonPressed);
            exitButton.onClick.RemoveListener(GameUIController.OnExitButtonPressed);
        }

        private void ShowOverlay() => _thisCanvas.enabled = true;

        private void HideOverlay() => _thisCanvas.enabled = false;
    }
}