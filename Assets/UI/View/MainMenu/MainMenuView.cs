using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.View.MainMenu {
    public class MainMenuView : MonoBehaviour {
        [SerializeField] private Button playButton;
        [SerializeField] private Button recordsButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private Button backButton;
        [SerializeField] private GameObject buttonContainer;
        [SerializeField] private GameObject recordContainer;
        [SerializeField] private TextMeshProUGUI recordText;
        
        public void OnEnable() => SubscribeToMethods();

        public void OnDisable() => UnsubscribeFromMethods();

        private void SubscribeToMethods() {
            playButton.onClick.AddListener(Play);
            exitButton.onClick.AddListener(Exit);
            recordsButton.onClick.AddListener(ShowRecords);
            backButton.onClick.AddListener(HideRecords);
        }

        private void UnsubscribeFromMethods() {
            playButton.onClick.RemoveListener(Play);
            exitButton.onClick.RemoveListener(Exit);
            recordsButton.onClick.RemoveListener(ShowRecords);
            backButton.onClick.RemoveListener(HideRecords);
        }

        private void ShowRecords() {
            buttonContainer.SetActive(false);
            recordContainer.SetActive(true);
            LoadRecords();
        }
        
        private void HideRecords() {
            recordContainer.SetActive(false);
            buttonContainer.SetActive(true);
        }

        private void LoadRecords() {
            var sessions = PlayerPrefs.GetString("Sessions");
            recordText.SetText(sessions);
        }

        private static void Play() => SceneManager.LoadScene("GameScene");
        // I don't know whether it work or not, because in unity it does nothing, but it may work in builds...
        private static void Exit() => Application.Quit();
    }
}