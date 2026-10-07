using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[DefaultExecutionOrder(-100)]
public class MainMenuManager : MonoBehaviour
{
    private string firstScene = "SampleScene";

    [Header("Screens")]
    [SerializeField] private Image screen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject optionsScreen;

    [Header("Settings")]
    [SerializeField] private TMP_Dropdown language;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;

    [Header("Sounds")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip selectEffect;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource effectsPlayer;

    [Header("Continue")]
    [SerializeField] private GameObject continueButton;

    private GameData data = new GameData();

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GetData();

        continueButton.SetActive(data.savedScene != "");

        StartCoroutine(EnterMenu());

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[data.language];

        audioMixer.SetFloat("MasterVolume", ToDb(data.masterVolume));
        audioMixer.SetFloat("MusicVolume", ToDb(data.musicVolume));
        audioMixer.SetFloat("EffectsVolume", ToDb(data.effectsVolume));

        musicPlayer.clip = menuMusic;
        musicPlayer.Play();
    }

    public void GetData()
    {
        data.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 50f);
        data.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        data.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        data.effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        data.savedScene = PlayerPrefs.GetString("SavedScene", "");
        data.language = PlayerPrefs.GetInt("Language", 0);
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("Sensitivity", data.sensitivity);
        PlayerPrefs.SetFloat("MasterVolume", data.masterVolume);
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
        sensitivitySlider.value = data.sensitivity;
        masterSlider.value = data.masterVolume;
        musicSlider.value = data.musicVolume;
        effectsSlider.value = data.effectsVolume;
    }

    public void SaveLanguage(int language)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language];
        data.language = language;
        SaveData();
    }

    public void SaveSensitivity(float sensitivity)
    {
        data.sensitivity = sensitivity;
        SaveData();
    }

    public void SaveMaster(float master)
    {
        audioMixer.SetFloat("MasterVolume", ToDb(master));
        data.masterVolume = master;
        SaveData();
    }

    public void SaveMusic(float music)
    {
        audioMixer.SetFloat("MusicVolume", ToDb(music));
        data.musicVolume = music;
        SaveData();
    }

    public void SaveEffects(float effects)
    {
        audioMixer.SetFloat("EffectsVolume", ToDb(effects));
        data.effectsVolume = effects;
        SaveData();
    }

    private float ToDb(float volume) => volume <= 0.0001f ? -80f : 20f * Mathf.Log10(volume);

    public void ClearData()
    {
        effectsPlayer.PlayOneShot(selectEffect);

        PlayerPrefs.DeleteAll();
        GetData();

        audioMixer.SetFloat("MasterVolume", ToDb(data.masterVolume));
        audioMixer.SetFloat("MusicVolume", ToDb(data.musicVolume));
        audioMixer.SetFloat("EffectsVolume", ToDb(data.effectsVolume));

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
