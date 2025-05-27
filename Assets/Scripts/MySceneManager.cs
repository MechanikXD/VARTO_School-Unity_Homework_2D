using UnityEngine;
using UnityEngine.SceneManagement;

/// Singleton class to manage current scene
public class MySceneManager : MonoBehaviour {
    private static MySceneManager _instance;
    private static Scene _currentScene;

    public static MySceneManager Instance {
        get {
            if (_instance != null) {
                return _instance;
            }

            var obj = new GameObject("MySceneManager");
            _instance = obj.AddComponent<MySceneManager>();
            DontDestroyOnLoad(obj);
            // Get active scene on instance create
            _currentScene = SceneManager.GetActiveScene();

            return _instance;
        }
    }
    
    public void ChangeAllColorsInScene() {
        foreach (var gameObj in _currentScene.GetRootGameObjects()) {
            if (gameObj.TryGetComponent<SpriteRenderer>(out var sprite)) {
                sprite.color = Random.ColorHSV();
            } 
        }
    }

}