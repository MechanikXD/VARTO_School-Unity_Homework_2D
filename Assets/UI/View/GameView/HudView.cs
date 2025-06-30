using TMPro;
using UI.Controllers;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.GameView {
    public class HudView : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI scoreCounter;
        [SerializeField] private TextMeshProUGUI heightCounter;
        [SerializeField] private Button pauseButton;

        public void Awake() {
            // Set initial value to counters
            scoreCounter.SetText("0");
            heightCounter.SetText("0");
        }
        
        public void OnEnable() => SubscribeToMethods();

        public void OnDisable() => UnsubscribeFromMethods();

        private void SubscribeToMethods() {
            pauseButton.onClick.AddListener(GameUIController.OnPauseButtonPressed);
            GameUIController.ScoreUpdateEvent += UpdateScoreCounter;
            GameUIController.HeightUpdateEvent += UpdateHeightCounter;
            
        }
        
        private void UnsubscribeFromMethods() {
            pauseButton.onClick.RemoveListener(GameUIController.OnPauseButtonPressed);
            GameUIController.ScoreUpdateEvent -= UpdateScoreCounter;
            GameUIController.HeightUpdateEvent -= UpdateHeightCounter;
        }

        private void UpdateScoreCounter() =>
            scoreCounter.SetText(SessionModel.CurrentScore.ToString());
        
        private void UpdateHeightCounter() =>
            heightCounter.SetText(SessionModel.CurrentHeight.ToString());
    }
}
