using TMPro;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View {
    public class HudView : MonoBehaviour {
        [SerializeField] private TMP_Text scoreCounter;
        [SerializeField] private TMP_Text heightCounter;
        [SerializeField] private Button pauseButton;

        public void OnEnable() {
            pauseButton.onClick.AddListener(PauseGame);
            UIController.ScoreUpdateEvent += UpdateScoreCounter;
            UIController.HeightUpdateEvent += UpdateHeightCounter;
        }

        public void Start() {
            scoreCounter.SetText("0");
            heightCounter.SetText("0");
        }

        public void OnDisable() {
            pauseButton.onClick.RemoveListener(PauseGame);
            UIController.ScoreUpdateEvent -= UpdateScoreCounter;
            UIController.HeightUpdateEvent -= UpdateHeightCounter;
        }
        
        private void UpdateScoreCounter() =>
            scoreCounter.SetText(SessionModel.CurrentScore.ToString());
        
        private void UpdateHeightCounter() =>
            heightCounter.SetText(SessionModel.CurrentHeight.ToString());

        private void PauseGame() {}
    }
}
