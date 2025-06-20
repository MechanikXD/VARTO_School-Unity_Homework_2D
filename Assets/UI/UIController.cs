using System;

namespace UI {
    public static class UIController {
        public static event Action ScoreUpdateEvent;
        public static event Action HeightUpdateEvent;

        public static void OnScoreUpdate() {
            ScoreUpdateEvent?.Invoke();
        }

        public static void OnHeightUpdate() {
            HeightUpdateEvent?.Invoke();
        }
    }
}