using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init() {
        var scene = SceneManager.GetActiveScene().name;
        if (scene == "MainMenu") {
            new GameObject("MainMenuUI").AddComponent<MainMenuUI>();
        } else if (scene == "LevelSelect") {
            new GameObject("LevelSelectUI").AddComponent<LevelSelectUI>();
        }
    }
}
