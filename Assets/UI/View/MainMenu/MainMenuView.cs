using UI.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.View.MainMenu {
    public class MainMenuView : MonoBehaviour {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        
        public void OnEnable() => SubscribeToMethods();

        public void OnDisable() => UnsubscribeFromMethods();

        private void SubscribeToMethods() {
            MainMenuController.PlayEvent += Play;
            MainMenuController.ExitEvent += Exit;
            
            playButton.onClick.AddListener(MainMenuController.OnPlay);
            exitButton.onClick.AddListener(MainMenuController.OnExit);
        }

        private void UnsubscribeFromMethods() {
            MainMenuController.PlayEvent -= Play;
            MainMenuController.ExitEvent -= Exit;
            
            playButton.onClick.RemoveListener(MainMenuController.OnPlay);
            exitButton.onClick.RemoveListener(MainMenuController.OnExit);
        }

        private void Play() => SceneManager.LoadScene("GameScene");
        // I don't know whether it work or not, because in unity it does nothing, but it may work in builds...
        public void Exit() => Application.Quit();
    }
}