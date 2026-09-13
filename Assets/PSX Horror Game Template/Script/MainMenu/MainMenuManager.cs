using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject optionsScreen;

    [Header("Settings")]
    [SerializeField] private SensitivitySlider sensitivity;
    [SerializeField] private EffectsSlider effects;
    [SerializeField] private MusicSlider music;

    [Header("Sounds")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip selectEffect;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource effectsPlayer;

    [Header("Continue")]
    [SerializeField] private GameObject continueButton;

    private void Awake()
    {
        ToStart();

        musicPlayer.volume = PlayerPrefs.GetFloat("Music", 100.0f) / 100.0f;
        effectsPlayer.volume = PlayerPrefs.GetFloat("Effects", 100.0f) / 100.0f;

        musicPlayer.clip = menuMusic;
        effectsPlayer.clip = selectEffect;

        musicPlayer.Play();
    }

    public void ToStart()
    {
        effectsPlayer.Play();
        
        startScreen.SetActive(true);
        optionsScreen.SetActive(false);
    }

    public void ToOptions()
    {
        effectsPlayer.Play();

        startScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void QuitGame()
    {
        effectsPlayer.Play();

        Application.Quit();
    }
}
