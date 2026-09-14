using System.Collections;
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

    private void Start()
    {
        continueButton.SetActive(MainManager.instance.data.savedScene != "");

        ToStart();
        StartCoroutine(EnterMenu());

        musicPlayer.volume = MainManager.instance.data.musicVolume;
        effectsPlayer.volume = MainManager.instance.data.effectsVolume;

        musicPlayer.clip = menuMusic;
        effectsPlayer.clip = selectEffect;

        musicPlayer.Play();

        ChangeLanguage(MainManager.instance.data.language);
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
        MainManager.instance.GetData();

        musicPlayer.volume = MainManager.instance.data.musicVolume;
        effectsPlayer.volume = MainManager.instance.data.effectsVolume;

        music.Get();
        effects.Get();
        sensitivity.Get();
        language.Get();

        continueButton.SetActive(false);

        ChangeLanguage(MainManager.instance.data.language);
    }

    public void ChangeLanguage(int index)
    {
        effectsPlayer.Play();

        StartCoroutine(ChangeLanguageCoroutine(index));
    }

    public void StartGame()
    {
        effectsPlayer.Play();

        StartCoroutine(StartGameCoroutine());
    }

    public void ContinueGame()
    {
        effectsPlayer.Play();

        StartCoroutine(ContinueGameCoroutine());
    }

    public void QuitGame()
    {
        effectsPlayer.Play();

        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator ChangeLanguageCoroutine(int index)
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return StartCoroutine(ExitMenu());
        MainManager.instance.data.savedScene = firstScene;
        MainManager.instance.SaveData();
        SceneManager.LoadScene(firstScene);
    }

    private IEnumerator ContinueGameCoroutine()
    {
        yield return StartCoroutine(ExitMenu());
        SceneManager.LoadScene(MainManager.instance.data.savedScene);
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
