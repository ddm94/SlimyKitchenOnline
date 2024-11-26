using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => 
        {
            GameManager.Instance.TogglePauseGame();
        });

        optionsButton.onClick.AddListener(() =>
        {
            Hide();

            OptionsUI.Instance.Show(Show);
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();

            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;

        Hide();
    }

    private void GameManager_OnLocalGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnLocalGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show() 
    { 
        gameObject.SetActive(true);

        // This will make this button selected when navigating the UI using a gamepad
        resumeButton.Select();
    }
    private void Hide() { gameObject.SetActive(false); }
}
