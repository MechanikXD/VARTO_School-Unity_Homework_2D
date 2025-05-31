using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Singleton class to manage current scene
public class MySceneManager : MonoBehaviour {
    private static MySceneManager _instance;
    private static Scene _currentScene;
    private GameObject _platformObject;

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

    public void SetupScene(Vector2 startPlatformCoordinates) {
        _platformObject = GameObject.FindGameObjectWithTag("Ground");
        if (_platformObject != null) {
            _platformObject.transform.position = startPlatformCoordinates;    
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