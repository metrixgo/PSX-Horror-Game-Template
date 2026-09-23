using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private string firstScene = "SampleScene";

    [Header("Screens")]
    [SerializeField] private Image screen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject optionsScreen;

    [Header("Settings")]
    [SerializeField] private TMP_Dropdown language;
    [SerializeField] private Slider sensitivity;
    [SerializeField] private Slider music;
    [SerializeField] private Slider effects;

    [Header("Sounds")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip selectEffect;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource effectsPlayer;

    [Header("Continue")]
    [SerializeField] private GameObject continueButton;

    private GameData data = new GameData();

    private void Awake()
    {
        GetData();

        continueButton.SetActive(data.savedScene != "");

        StartCoroutine(EnterMenu());

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[data.language];

        musicPlayer.volume = data.musicVolume;
        effectsPlayer.volume = data.effectsVolume;

        musicPlayer.clip = menuMusic;
        musicPlayer.Play();
    }

    public void GetData()
    {
        data.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 100f);
        data.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        data.effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        data.savedScene = PlayerPrefs.GetString("SavedScene", "");
        data.language = PlayerPrefs.GetInt("Language", 0);
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("Sensitivity", data.sensitivity);
        PlayerPrefs.SetFloat("MusicVolume", data.musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", data.effectsVolume);
        PlayerPrefs.SetString("SavedScene", data.savedScene);
        PlayerPrefs.SetInt("Language", data.language);

        PlayerPrefs.Save();
    }

    public void ToStart()
    {
        effectsPlayer.PlayOneShot(selectEffect);
        
        startScreen.SetActive(true);
        optionsScreen.SetActive(false);
    }

    public void ToOptions()
    {
        effectsPlayer.PlayOneShot(selectEffect);

        UpdateOptions();

        startScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void UpdateOptions()
    {
        language.value = data.language;
        sensitivity.value = data.sensitivity;
        music.value = data.musicVolume;
        effects.value = data.effectsVolume;
    }

    public void SaveLanguage(int language)
    {
        data.language = language;
        SaveData();
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language];
    }

    public void SaveSensitivity(float sensitivity)
    {
        data.sensitivity = sensitivity;
        SaveData();
    }

    public void SaveMusic(float music)
    {
        musicPlayer.volume = music;
        data.musicVolume = music;
        SaveData();
    }

    public void SaveEffects(float effects)
    {
        effectsPlayer.volume = effects;
        data.effectsVolume = effects;
        SaveData();
    }

    public void ClearData()
    {
        effectsPlayer.PlayOneShot(selectEffect);

        PlayerPrefs.DeleteAll();
        GetData();

        musicPlayer.volume = data.musicVolume;
        effectsPlayer.volume = data.effectsVolume;

        UpdateOptions();

        continueButton.SetActive(false);

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[data.language];
    }

    public void StartGame()
    {
        effectsPlayer.PlayOneShot(selectEffect);

        StartCoroutine(StartGameCoroutine());
    }

    public void ContinueGame()
    {
        effectsPlayer.PlayOneShot(selectEffect);

        StartCoroutine(ContinueGameCoroutine());
    }

    public void QuitGame()
    {
        effectsPlayer.PlayOneShot(selectEffect);

        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return StartCoroutine(ExitMenu());
        data.savedScene = firstScene;
        SaveData();
        SceneManager.LoadScene(firstScene);
    }

    private IEnumerator ContinueGameCoroutine()
    {
        yield return StartCoroutine(ExitMenu());
        SceneManager.LoadScene(data.savedScene);
    }

    private IEnumerator QuitGameCoroutine()
    {
        yield return StartCoroutine(ExitMenu());
        Application.Quit();
    }

    private IEnumerator EnterMenu()
    {
        screen.raycastTarget = true;
        screen.color = Color.black;

        yield return LocalizationSettings.InitializationOperation;

        float t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            screen.color = Color.Lerp(Color.black, Color.clear, t / 2f);
            yield return null;
        }

        screen.color = Color.clear;
        screen.raycastTarget = false;
    }

    private IEnumerator ExitMenu()
    {
        screen.raycastTarget = true;
        screen.color = Color.clear;

        float t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            screen.color = Color.Lerp(Color.clear, Color.black, t / 2f);
            yield return null;
        }

        screen.color = Color.black;
        screen.raycastTarget = false;
    }

}
