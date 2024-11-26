using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // () => Lambda Expression where () is to define the params of the function, which in this case are none
        playButton.onClick.AddListener(() =>
        {
            // Click
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        });

        // This ensures that if we pause the game and go back to the main menu, the time scale will reset to normal.
        Time.timeScale = 1f;
    }
}
