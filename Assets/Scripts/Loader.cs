using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        // These MUST match the actual name of the actual scene
        MainMenuScene,
        GameScene,
        LoadingScene,
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    // Wait one Update() to make sure the LoadingScene properly gets rendered before loading the next scene
    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
