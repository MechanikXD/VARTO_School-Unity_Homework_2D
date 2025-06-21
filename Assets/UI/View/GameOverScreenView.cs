using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View {
    public class GameOverScreenView : MonoBehaviour {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private TMP_Text currentScoreText;
        [SerializeField] private TMP_Text bestScoreText;
        [SerializeField] private TMP_Text currentHeightText;
        [SerializeField] private TMP_Text bestHeightText;
    }
}