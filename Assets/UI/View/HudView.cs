using Core;
using TMPro;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View {
    public class HudView : MonoBehaviour {
        [SerializeField] private TMP_Text scoreCounter;
        [SerializeField] private TMP_Text heightCounter;
        [SerializeField] private Button pauseButton;

        public void OnEnable() => SubscribeToMethods();
        
        public void Start() {
            // Set initial value to counters
            scoreCounter.SetText("0");
            heightCounter.SetText("0");
        }

        public void OnDisable() => UnsubscribeFromMethods();

        private void SubscribeToMethods() {
            pauseButton.onClick.AddListener(UIController.OnPauseButtonPressed);
            UIController.ScoreUpdateEvent += UpdateScoreCounter;
            UIController.HeightUpdateEvent += UpdateHeightCounter;
            UIController.PauseButtonPressedEvent += MySceneManager.Instance.PauseGameSession;
            UIController.ResumeButtonPressedEvent += MySceneManager.Instance.ResumeGameSession;
        }
        
        private void UnsubscribeFromMethods() {
            pauseButton.onClick.RemoveListener(UIController.OnPauseButtonPressed);
            UIController.ScoreUpdateEvent -= UpdateScoreCounter;
            UIController.HeightUpdateEvent -= UpdateHeightCounter;
            UIController.PauseButtonPressedEvent -= MySceneManager.Instance.PauseGameSession;
            UIController.ResumeButtonPressedEvent -= MySceneManager.Instance.ResumeGameSession;
        }

        private void UpdateScoreCounter() =>
            scoreCounter.SetText(SessionModel.CurrentScore.ToString());
        
        private void UpdateHeightCounter() =>
            heightCounter.SetText(SessionModel.CurrentHeight.ToString());
    }
}
