using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject optionsScreen;

    [Header("Settings")]
    [SerializeField] private SensitivitySlider sensitivity;
    [SerializeField] private MusicSlider music;
    [SerializeField] private EffectsSlider effects;
    [SerializeField] private LanguageDropdown language;

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

        musicPlayer.volume = PlayerPrefs.GetFloat("Music", 1f);
        effectsPlayer.volume = PlayerPrefs.GetFloat("Effects", 1f);

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

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void StartGame()
    {

    }

    public void ContinueGame()
    {

    }

    public void QuitGame()
    {
        effectsPlayer.Play();

        Application.Quit();
    }
}
