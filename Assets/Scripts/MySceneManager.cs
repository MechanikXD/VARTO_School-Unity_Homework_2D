using UnityEngine;

public class MySceneManager : MonoBehaviour {
    private static MySceneManager _instance;

    public static MySceneManager Instance {
        get {
            if (_instance != null) {
                return _instance;
            }

            var obj = new GameObject("MySceneManager");
            _instance = obj.AddComponent<MySceneManager>();
            DontDestroyOnLoad(obj);

            return _instance;
        }
    }

    public void ChangeAllColorsInScene() {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach (var gameObj in currentScene.GetRootGameObjects()) {
            if (gameObj.TryGetComponent<SpriteRenderer>(out var sprite)) {
                sprite.color = Random.ColorHSV();
            } 
        }
    }

}