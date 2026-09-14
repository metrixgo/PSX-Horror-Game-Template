using System;
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

    private void Start()
    {
        continueButton.SetActive(MainManager.instance.data.savedScene != "");

        StartCoroutine(EnterMenu());

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[MainManager.instance.data.language];

        musicPlayer.volume = MainManager.instance.data.musicVolume;
        effectsPlayer.volume = MainManager.instance.data.effectsVolume;

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

        UpdateOptions();

        startScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void UpdateOptions()
    {
        language.value = MainManager.instance.data.language;
        sensitivity.value = MainManager.instance.data.sensitivity;
        music.value = MainManager.instance.data.musicVolume;
        effects.value = MainManager.instance.data.effectsVolume;
    }

    public void SaveLanguage(int language)
    {
        MainManager.instance.data.language = language;
        MainManager.instance.SaveData();
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language];
    }

    public void SaveSensitivity(float sensitivity)
    {
        MainManager.instance.data.sensitivity = sensitivity;
        MainManager.instance.SaveData();
    }

    public void SaveMusic(float music)
    {
        musicPlayer.volume = music;
        MainManager.instance.data.musicVolume = music;
        MainManager.instance.SaveData();
    }

    public void SaveEffects(float effects)
    {
        effectsPlayer.volume = effects;
        MainManager.instance.data.effectsVolume = effects;
        MainManager.instance.SaveData();
    }

    public void ClearData()
    {
        effectsPlayer.Play();

        PlayerPrefs.DeleteAll();
        MainManager.instance.GetData();

        musicPlayer.volume = MainManager.instance.data.musicVolume;
        effectsPlayer.volume = MainManager.instance.data.effectsVolume;

        UpdateOptions();

        continueButton.SetActive(false);

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[MainManager.instance.data.language];
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
