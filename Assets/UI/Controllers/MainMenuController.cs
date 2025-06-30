using System;

namespace UI.Controllers {
    public static class MainMenuController {
        public static event Action PlayEvent;
        public static event Action SettingsOpenEvent;
        public static event Action ExitEvent;

        public static void OnPlay() => PlayEvent?.Invoke();
        public static void OnSettingsOpen() => SettingsOpenEvent?.Invoke();
        public static void OnExit() => ExitEvent?.Invoke();
    }
}