using System;

namespace UI.Controllers {
    public static class GameUIController {
        public static event Action ScoreUpdateEvent;
        public static event Action HeightUpdateEvent;
        public static event Action PauseButtonPressedEvent;
        public static event Action ResumeButtonPressedEvent;
        public static event Action RestartButtonPressedEvent;
        public static event Action ExitButtonPressedEvent;
        public static event Action GameOverEvent;

        public static void OnScoreUpdate() => ScoreUpdateEvent?.Invoke();
        public static void OnHeightUpdate() => HeightUpdateEvent?.Invoke();
        

        public static void OnPauseButtonPressed() => PauseButtonPressedEvent?.Invoke();
        public static void OnResumeButtonPressed() => ResumeButtonPressedEvent?.Invoke();
        public static void OnRestartButtonPressed() => RestartButtonPressedEvent?.Invoke();
        public static void OnExitButtonPressed() => ExitButtonPressedEvent?.Invoke();

        public static void OnGameOver() {
            GameOverEvent?.Invoke();
        }
    }
}