using UnityEngine;


public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance {get; private set; }

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    private AudioSource audioSource;

    private float volume = .3f;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("There is more than one MusicManager instance.");

        Instance = this;

        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        audioSource.volume = volume;
    }

    public void ChangeVolume()
    {
        volume += .1f;

        if (volume > 1.1f)
        {
            volume = 0f;
        }

        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
