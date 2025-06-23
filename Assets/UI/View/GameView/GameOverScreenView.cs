using TMPro;
using UI.Controllers;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.GameView {
    public class GameOverScreenView : MonoBehaviour {
        private Canvas _thisCanvas;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private TMP_Text currentScoreText;
        [SerializeField] private TMP_Text bestScoreText;
        [SerializeField] private TMP_Text currentHeightText;
        [SerializeField] private TMP_Text bestHeightText;
        
        public void OnEnable() => SubscribeToEvents();
        public void Start() {
            TryGetComponent(out _thisCanvas);                    // Cash attached canvas
            HideOverlay();                                       // Hide on scene load
        }

        public void OnDisable() => UnsubscribeFromEvents();
        
        private void SubscribeToEvents() {
            GameUIController.GameOverEvent += ShowOverlay;
            
            restartButton.onClick.AddListener(GameUIController.OnRestartButtonPressed);
            exitButton.onClick.AddListener(GameUIController.OnExitButtonPressed);
        }

        private void UnsubscribeFromEvents() {
            GameUIController.GameOverEvent -= ShowOverlay;
            
            restartButton.onClick.RemoveListener(GameUIController.OnRestartButtonPressed);
            exitButton.onClick.RemoveListener(GameUIController.OnExitButtonPressed);
        }

        private void ShowOverlay() {
            currentScoreText.SetText(SessionModel.CurrentScore.ToString());
            bestScoreText.SetText(SessionModel.BestScore.ToString());
            currentHeightText.SetText(SessionModel.CurrentHeight.ToString());
            bestHeightText.SetText(SessionModel.BestHeight.ToString());
            
            _thisCanvas.enabled = true;
        }

        private void HideOverlay() => _thisCanvas.enabled = false;
    }
}