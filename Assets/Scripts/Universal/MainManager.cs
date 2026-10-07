using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameData
{
    public int language;
    public float sensitivity;
    public float masterVolume;
    public float musicVolume;
    public float effectsVolume;
    public string savedScene;
}

[DefaultExecutionOrder(-100)]
public class MainManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Sound Players")]
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource effectsPlayer;
    [SerializeField] private AudioSource writingEffectsPlayer;
    [SerializeField] private AudioSource buttonEffectsPlayer;

    [Header("Sounds")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip backGroundMusic;
    [SerializeField] private AudioClip writingEffect;
    [SerializeField] private AudioClip endingEffect;
    [SerializeField] private AudioClip selectEffect;

    [Header("Pause")]
    [SerializeField] private GameObject pausedScreen;
    [SerializeField] private TMP_Dropdown language;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueScreen;
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    [SerializeField] private TextMeshProUGUI dialogueContent;
    [SerializeField] private GameObject subdialogueScreen;
    [SerializeField] private TextMeshProUGUI subdialogueSpeaker;
    [SerializeField] private TextMeshProUGUI subdialogueContent;
    private float[] displayGap = { 0.02f, 0.04f };

    [Header("Screen")]
    [SerializeField] private Image subscreen;
    [SerializeField] private Image screen;
    [SerializeField] private Image superscreen;

    [Header("Prompts")]
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private TextMeshProUGUI subprompt;
    private Color promptColor = Color.white;
    private Color subpromptColor = Color.white;
    private bool flashPrompt = false;
    private float flashPromptT = 0f;
    private bool flashSubprompt = false;
    private float flashSubpromptT = 0f;

    [Header("Tasks")]
    [SerializeField] private TextMeshProUGUI tasksPrompt;
    private List<string> tasks = new List<string>();

    [Header("Ending")]
    [SerializeField] private GameObject endingScreen;
    [SerializeField] private TextMeshProUGUI endingTitle;
    [SerializeField] private TextMeshProUGUI endingDescription;
    [SerializeField] private GameObject endingReturnMenuButton;

    [Header("Start Trigger")]
    [SerializeField] private List<Trigger> startTriggers = new List<Trigger>();

    public static MainManager Instance { get; private set; }

    public GameData Data { get; private set; } = new GameData();
    public int PlayerBlockLayers { get; private set; } = 0;
    public bool IsExecutingTriggers { get; private set; } = false;
    public bool IsPaused { get; private set; } = false;

    private int pauseBlockLayers = 0;

    private string mainMenuName = "MainMenu";

    private string translationTableName = "TranslationTable";

    private GameInput input;
    private InputAction pauseAction;
    private InputAction returnAction;
    private InputAction skipAction;
    private bool pauseInput;
    private bool returnInput;
    private bool skipInput;

    private List<Trigger> triggers = new List<Trigger>();
    private List<string> inventory = new List<string>();

    private void Awake()
    {
        Instance = this;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        input = new GameInput();
        pauseAction = input.Game.Pause;
        returnAction = input.Game.Return;
        skipAction = input.Game.Skip;

        GetData();

        language.value = Data.language;
        sensitivitySlider.value = Data.sensitivity;
        masterSlider.value = Data.masterVolume;
        musicSlider.value = Data.musicVolume;
        effectsSlider.value = Data.effectsVolume;
        audioMixer.SetFloat("MasterVolume", ToDb(Data.masterVolume));
        audioMixer.SetFloat("MusicVolume", ToDb(Data.musicVolume));
        audioMixer.SetFloat("EffectsVolume", ToDb(Data.effectsVolume));

        writingEffectsPlayer.clip = writingEffect;
        buttonEffectsPlayer.ignoreListenerPause = true;

        PlayMusic(backGroundMusic);

        foreach (Trigger trig in startTriggers)
            triggers.Add(trig);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        GetInput();
        CheckPause();

        if (IsPaused) return;

        UpdatePrompts();

        if (!IsExecutingTriggers && triggers.Count > 0)
            StartCoroutine(ExecuteTriggers());
    }

    private void GetInput()
    {
        pauseInput = pauseAction.WasPressedThisFrame();
        returnInput = returnAction.WasPressedThisFrame();
        skipInput = skipAction.WasPressedThisFrame();
    }

    private void CheckPause()
    {
        if (!pauseInput || pauseBlockLayers > 0) return;

        if (IsPaused) Resume();
        else Pause();
    }

    private void UpdatePrompts()
    {
        if (flashPrompt)
        {
            flashPromptT += Time.deltaTime;
            prompt.color = Color.Lerp(Color.clear, promptColor, (Mathf.Cos(flashPromptT * 5f) + 1f) / 2f * 0.75f + 0.25f); ;
        }
        else
        {
            flashPromptT = 0;
            prompt.color = promptColor;
        }

        if (flashSubprompt)
        {
            flashSubpromptT += Time.deltaTime;
            subprompt.color = Color.Lerp(Color.clear, subpromptColor, (Mathf.Cos(flashSubpromptT * 5f) + 1f) / 2f * 0.75f + 0.25f);
        }
        else
        {
            flashSubpromptT = 0;
            subprompt.color = subpromptColor;
        }
    }

    public void Pause()
    {
        if (pauseBlockLayers > 0 || IsPaused) return;

        IsPaused = true;
        pausedScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Resume()
    {
        if (pauseBlockLayers > 0 || !IsPaused) return;

        IsPaused = false;
        pausedScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    public void SaveLanguage(int language)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language];
        Data.language = language;
        SaveData();
    }

    public void SaveSensitivity(float sensitivity)
    {
        Data.sensitivity = sensitivity;
        SaveData();
    }

    public void SaveMaster(float master)
    {
        audioMixer.SetFloat("MasterVolume", ToDb(master));
        Data.masterVolume = master;
        SaveData();
    }

    public void SaveMusic(float music)
    {
        audioMixer.SetFloat("MusicVolume", ToDb(music));
        Data.musicVolume = music;
        SaveData();
    }

    public void SaveEffects(float effects)
    {
        audioMixer.SetFloat("EffectsVolume", ToDb(effects));
        Data.effectsVolume = effects;
        SaveData();
    }

    private float ToDb(float volume) => volume <= 0.0001f ? -80f : 20f * Mathf.Log10(volume);

    public void GetData()
    {
        Data.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 50f);
        Data.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        Data.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        Data.effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        Data.savedScene = PlayerPrefs.GetString("SavedScene", "");
        Data.language = PlayerPrefs.GetInt("Language", 0);
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("Sensitivity", Data.sensitivity);
        PlayerPrefs.SetFloat("MasterVolume", Data.masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", Data.musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", Data.effectsVolume);
        PlayerPrefs.SetString("SavedScene", Data.savedScene);
        PlayerPrefs.SetInt("Language", Data.language);

        PlayerPrefs.Save();
    }

    public void AddTrigger(Trigger trigger)
    {
        triggers.Add(trigger);
    }

    public string Translate(string s)
    {
        StringTable table = LocalizationSettings.StringDatabase.GetTable(translationTableName);

        if (table != null && table.GetEntry(s) != null)
            return LocalizationSettings.StringDatabase.GetLocalizedString(translationTableName, s);
        else
            return s;
    }

    public void SetPrompt(string prompt, Color color, bool sub, bool flash)
    {
        prompt = Translate(prompt);

        if (sub)
        {
            subprompt.text = prompt;
            subpromptColor = color;
            flashSubprompt = flash;
            flashSubpromptT = 0;
        }
        else
        {
            this.prompt.text = prompt;
            promptColor = color;
            flashPrompt = flash;
            flashPromptT = 0;
        }
    }

    public void AddTask(string s)
    {
        tasks.Add(s);
        UpdateTask();
    }

    public void RemoveTask(string s)
    {
        tasks.Remove(s);
        UpdateTask();
    }

    public void ClearTasks()
    {
        tasks.Clear();
        UpdateTask();
    }

    public void UpdateTask()
    {
        string s = "";
        foreach (string task in tasks)
        {
            s += "- " + Translate(task) + "\n";
        }
        tasksPrompt.text = s;
    }

    public void AddItem(string s)
    {
        inventory.Add(s);
    }

    public void RemoveItem(string s)
    {
        inventory.Remove(s);
    }

    public bool HasItem(string s)
    {
        return inventory.Contains(s);
    }

    public int ItemCount(string s)
    {
        int cnt = 0;
        foreach (string item in inventory)
        {
            if (s == item) cnt++;
        }
        return cnt;
    }

    public void BlockPlayer(bool b)
    {
        if(b) PlayerBlockLayers++;
        else PlayerBlockLayers = Mathf.Max(0, PlayerBlockLayers - 1);
    }

    public void BlockPause(bool b)
    {
        if (b) pauseBlockLayers++;
        else pauseBlockLayers = Mathf.Max(pauseBlockLayers - 1, 0);
    }

    public void PlayMusic(AudioClip music)
    {
        musicPlayer.clip = music;
        musicPlayer.Play();
    }

    public void PlayEffect(AudioClip effect)
    {
        effectsPlayer.PlayOneShot(effect);
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
    }

    private IEnumerator ExecuteTriggers()
    {
        BlockPlayer(true);
        IsExecutingTriggers = true;

        while (triggers.Count > 0)
        {
            Trigger trig = triggers[0];
            triggers.RemoveAt(0);

            switch (trig.triggerType)
            {
                case TriggerType.DisplayDialogue:
                    yield return StartCoroutine(
                        DisplayDialogue(
                            trig.displayDialogueSpeaker,
                            trig.displayDialogueContent,
                            trig.displayDialogueSub,
                            trig.displayDialogueFlash,
                            trig.displayDialogueSkippable,
                            trig.displayDialogueFlashLength
                        )
                    );
                    break;

                case TriggerType.ChangeScreen:

                    IEnumerator changeScreenCoroutine =
                        ChangeScreen(
                            trig.changeScreenStartColor,
                            trig.changeScreenEndColor,
                            trig.changeScreenLength,
                            trig.changeScreenSub,
                            trig.changeScreenFlash
                        );

                    if (trig.changeScreenWaitForCompletion || trig.changeScreenFlash)
                        yield return StartCoroutine(changeScreenCoroutine);
                    else
                        StartCoroutine(changeScreenCoroutine);

                    break;

                case TriggerType.Wait:
                    if (trig.waitFlash) BlockPlayer(false);
                    yield return new WaitForSeconds(trig.waitLength);
                    if (trig.waitFlash) BlockPlayer(true);
                    break;

                case TriggerType.DisplayPrompt:
                    SetPrompt(trig.displayPromptPrompt, trig.displayPromptColor, trig.displayPromptSub, trig.displayPromptFlash);
                    break;

                case TriggerType.ManageTasks:
                    switch (trig.manageTasksType)
                    {
                        case ManageTasksType.AddTask:
                            AddTask(trig.manageTasksTask);
                            break;
                        case ManageTasksType.RemoveTask:
                            RemoveTask(trig.manageTasksTask);
                            break;
                        case ManageTasksType.ClearTasks:
                            ClearTasks();
                            break;
                        default:
                            Debug.LogError("Unimplemented Manage Tasks Type: " + trig.manageTasksType);
                            break;
                    }
                    break;

                case TriggerType.ManageInventory:
                    if (trig.manageInventoryAddItem) AddItem(trig.manageInventoryItem);
                    else RemoveItem(trig.manageInventoryItem);
                    break;

                case TriggerType.PlayerCanDo:
                    switch (trig.playerCanDoType)
                    {
                        case PlayerCanDoType.Look:
                            player.CanLook(trig.playerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Move:
                            player.CanMove(trig.playerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Sprint:
                            player.CanSprint(trig.playerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Jump:
                            player.CanJump(trig.playerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Crouch:
                            player.CanCrouch(trig.playerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Interact:
                            player.CanInteract(trig.playerCanDoCanDo);
                            break;
                        default:
                            Debug.LogError("Unimplemented Player Can Do Type: " + trig.playerCanDoType);
                            break;
                    }
                    break;

                case TriggerType.MovePlayer:
                    switch (trig.movePlayerType)
                    {
                        case MovePlayerType.Location:
                            player.SetPosition(trig.movePlayerVector);
                            break;
                        case MovePlayerType.Direction:
                            player.Move(trig.movePlayerVector);
                            break;
                        default:
                            Debug.LogError("Unimplemented Move Player Type: " + trig.movePlayerType);
                            break;
                    }
                    break;

                case TriggerType.JumpscareAt:
                    player.LookAt(trig.jumpscareAtObject.position, trig.jumpscareAtLength);
                    PlayEffect(trig.jumpscareAtEffect);
                    yield return new WaitForSeconds(trig.jumpscareAtLength);
                    break;

                case TriggerType.DisplayCanvas:
                    yield return StartCoroutine(
                        DisplayCanvas(
                            trig.displayCanvasCanvas,
                            trig.displayCanvasEffect,
                            trig.displayCanvasFlash,
                            trig.displayCanvasFlashLength
                        )
                    );
                    break;

                case TriggerType.PlaySound:
                    if (trig.playSoundLocal)
                    {
                        trig.playSoundSource.clip = trig.playSoundSound;
                        trig.playSoundSource.Play();
                    }
                    else
                    {
                        if (trig.playSoundIsEffect) PlayEffect(trig.playSoundSound);
                        else PlayMusic(trig.playSoundSound);
                    }
                    break;

                case TriggerType.SetObject:
                    trig.setObjectObject.SetActive(trig.setObjectSetActive);
                    break;

                case TriggerType.LoadScene:
                    yield return StartCoroutine(
                        LoadScene(
                            trig.loadSceneScene,
                            trig.loadSceneLength,
                            trig.loadSceneSave
                        )
                    );
                    break;

                case TriggerType.DisplayEnding:
                    yield return StartCoroutine(
                        DisplayEnding(
                            trig.displayEndingTitle,
                            trig.displayEndingDescription
                        )
                    );
                    break;

                case TriggerType.GlitchEffect:
                    switch (trig.glitchEffectType)
                    {
                        case GlitchEffectType.DigitalGlitch:
                            player.SetDigitalGlitch(trig.glitchEffectIntensity);
                            break;
                        case GlitchEffectType.ScanLineJitter:
                            player.SetScanLineJitter(trig.glitchEffectIntensity);
                            break;
                        case GlitchEffectType.VerticalJump:
                            player.SetVerticalJump(trig.glitchEffectIntensity);
                            break;
                        case GlitchEffectType.HorizontalShake:
                            player.SetHorizontalShake(trig.glitchEffectIntensity);
                            break;
                        case GlitchEffectType.ColorDrift:
                            player.SetColorDrift(trig.glitchEffectIntensity);
                            break;
                        case GlitchEffectType.HorizontalRipple:
                            player.SetHorizontalRipple(trig.glitchEffectIntensity);
                            break;
                        default:
                            Debug.LogError("Unimplemented Glitch Effect Type: " + trig.glitchEffectType);
                            break;
                    }
                    break;

                case TriggerType.Custom:
                    trig.customFunction.Invoke();
                    break;

                default:
                    Debug.LogError("Trigger Not Found: " + trig.triggerType);
                    break;
            }
        }
        BlockPlayer(false);
        IsExecutingTriggers = false;
    }

    private IEnumerator DisplayDialogue(string speaker, string content, bool sub, bool flash, bool skippable, float flashLength)
    {
        if (flash) BlockPlayer(false);

        writingEffectsPlayer.Play();

        TextMeshProUGUI targetSpeaker = sub ? subdialogueSpeaker : dialogueSpeaker;
        TextMeshProUGUI targetContent = sub ? subdialogueContent : dialogueContent;
        GameObject targetScreen = sub ? subdialogueScreen : dialogueScreen;

        speaker = Translate(speaker);
        content = Translate(content);
        targetSpeaker.text = speaker;
        targetContent.text = content;
        targetContent.maxVisibleCharacters = 0;
        targetScreen.SetActive(true);

        targetContent.ForceMeshUpdate();
        int contentLength = targetContent.textInfo.characterCount;
        float t = 0, gap = displayGap[Data.language];
        while (targetContent.maxVisibleCharacters < contentLength)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                targetContent.maxVisibleCharacters++;
            }
            if ((skipInput && skippable && !IsPaused))
            {
                targetContent.maxVisibleCharacters = int.MaxValue;
                break;
            }
            yield return null;
        }

        writingEffectsPlayer.Stop();
        targetContent.maxVisibleCharacters = int.MaxValue;

        yield return new WaitForSeconds(0.05f);
        if (flash) yield return new WaitForSeconds(flashLength);
        else yield return new WaitUntil(() => skipInput && !IsPaused);

        targetScreen.SetActive(false);

        if (flash) BlockPlayer(true);
    }

    private IEnumerator ChangeScreen(Color startColor, Color endColor, float length, bool sub, bool flash)
    {
        if (flash) BlockPlayer(false);

        Image targetScreen = sub ? subscreen : screen;

        targetScreen.color = startColor;
        float t = 0;
        while (t < length)
        {
            yield return null;
            t += Time.deltaTime;
            targetScreen.color = Color.Lerp(startColor, endColor, t / length);
        }
        targetScreen.color = endColor;

        if (flash) BlockPlayer(true);
    }

    private IEnumerator DisplayCanvas(GameObject canvas, AudioClip effect, bool flash, float flashLength)
    {
        if (flash) BlockPlayer(false);
        else BlockPause(true);

        canvas.SetActive(true);
        PlayEffect(effect);

        if (flash)
            yield return new WaitForSeconds(flashLength);
        else
            yield return new WaitUntil(() => returnInput);

        canvas.SetActive(false);
        PlayEffect(effect);

        if (flash) BlockPlayer(true);
        else BlockPause(false);
    }

    private IEnumerator LoadScene(string scene, float length, bool save)
    {
        if (save)
        {
            Data.savedScene = scene;
            SaveData();
        }

        yield return StartCoroutine(ChangeScreen(Color.clear, Color.black, length, false, false));

        SceneManager.LoadScene(scene);
    }

    private IEnumerator LoadMainMenu()
    {
        buttonEffectsPlayer.PlayOneShot(selectEffect);

        BlockPause(true);
        superscreen.raycastTarget = true;
        superscreen.color = Color.clear;

        float t = 0f;
        while (t < 2f)
        {
            yield return null;
            t += Time.unscaledDeltaTime;
            superscreen.color = Color.Lerp(Color.clear, Color.black, t / 2f);
        }

        SceneManager.LoadScene(mainMenuName);
    }

    private IEnumerator DisplayEnding(string title, string description)
    {
        BlockPause(true);
        IsPaused = true;

        writingEffectsPlayer.Play();

        screen.color = Color.black;
        superscreen.color = Color.clear;

        title = Translate(title);
        description = Translate(description);
        endingTitle.text = "";
        endingDescription.text = description;
        endingDescription.maxVisibleCharacters = 0;
        endingReturnMenuButton.SetActive(false);
        endingScreen.SetActive(true);

        endingDescription.ForceMeshUpdate();
        int endingDescriptionLength = endingDescription.textInfo.characterCount;
        float t = 0, gap = displayGap[Data.language];

        while (endingDescription.maxVisibleCharacters < endingDescriptionLength)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                endingDescription.maxVisibleCharacters++;
            }
            if (skipInput)
            {
                endingDescription.maxVisibleCharacters = endingDescriptionLength;
                break;
            }
            yield return null;
        }
        endingDescription.maxVisibleCharacters = endingDescriptionLength;
        writingEffectsPlayer.Stop();

        yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => (skipInput));
        yield return new WaitForSeconds(0.05f);
        writingEffectsPlayer.Play();

        t = 0;
        gap /= 10f;
        while (endingDescription.maxVisibleCharacters >= 0)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                endingDescription.maxVisibleCharacters--;
            }
            if (skipInput)
            {
                endingDescription.maxVisibleCharacters = 0;
                break;
            }
            yield return null;
        }
        endingDescription.maxVisibleCharacters = 0;
        writingEffectsPlayer.Stop();

        yield return new WaitForSeconds(1f);
        endingTitle.text = title;
        writingEffectsPlayer.PlayOneShot(endingEffect);

        yield return new WaitForSeconds(1f);
        endingReturnMenuButton.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}