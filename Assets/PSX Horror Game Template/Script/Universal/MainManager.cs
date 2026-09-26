using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Normal,
    Paused,
}

public class GameData
{
    public float sensitivity;
    public float musicVolume;
    public float effectsVolume;
    public string savedScene;
    public int language;
}

public class MainManager : MonoBehaviour
{
    public static MainManager instance { get; private set; }

    public GameState gameState { get; private set; } = GameState.Normal;

    public GameData data { get; private set; } = new GameData();

    public bool IsPlayerActive { get; private set; } = true;
    public bool IsExecutingTriggers { get; private set; } = false;
    public bool IsPaused { get; private set; } = false;
    public bool IsAtEnding { get; private set; } = false;

    private bool CanPause = true;

    private string mainMenuName = "MainMenu";

    private InputSystem input;

    private InputAction returnAction;
    private InputAction skipAction;

    private bool returnInput;
    private bool skipInput;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Sounds")]
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource effectsPlayer;

    [Header("Pause")]
    [SerializeField] private GameObject pausedScreen;
    [SerializeField] private Image pausedFrontScreen;
    [SerializeField] private Slider sensitivity;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueScreen;
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    [SerializeField] private TextMeshProUGUI dialogueContent;
    [SerializeField] private GameObject subdialogueScreen;
    [SerializeField] private TextMeshProUGUI subdialogueSpeaker;
    [SerializeField] private TextMeshProUGUI subdialogueContent;
    private AudioClip writingEffect;
    private float[] displayGap = { 0.02f, 0.04f };

    [Header("Screen")]
    [SerializeField] private Image screen;
    [SerializeField] private Image subscreen;

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
    [SerializeField] private Image endingFrontScreen;
    [SerializeField] private TextMeshProUGUI endingTitle;
    [SerializeField] private TextMeshProUGUI endingDescription;
    [SerializeField] private GameObject endingReturnMenuButton;
    private AudioClip endingEffect;

    private List<Trigger> triggers = new List<Trigger>();

    private List<string> inventory = new List<string>();

    private void Awake()
    {
        instance = this;

        input = new InputSystem();

        returnAction = input.Game.Return;
        skipAction = input.Game.Skip;

        GetData();

        sensitivity.value = data.sensitivity;
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
        if (!IsExecutingTriggers && triggers.Count > 0 && gameState == GameState.Normal)
            StartCoroutine(ExecuteTriggers());

        GetInput();
        UpdatePrompts();
        CheckPause();
    }

    private void GetInput()
    {
        returnInput = returnAction.WasPressedThisFrame();
        skipInput = skipAction.WasPressedThisFrame();
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

    private void CheckPause()
    {
        if (!returnInput || !CanPause) return;

        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (!CanPause || IsPaused) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pausedScreen.SetActive(true);
        Time.timeScale = 0f;

        IsPaused = true;
    }

    public void Resume()
    {
        if (!CanPause || !IsPaused) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pausedScreen.SetActive(false);
        Time.timeScale = 1f;

        IsPaused = false;
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    public void ChangeSensitivity(float sensitivity)
    {
        data.sensitivity = sensitivity;
        SaveData();
    }

    public void GetData()
    {
        data.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 50f);
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

    public void AddTrigger(Trigger trigger)
    {
        triggers.Add(trigger);
    }

    public string Translate(string s)
    {
        StringTable table = LocalizationSettings.StringDatabase.GetTable("TranslationTable");

        if (table != null && table.GetEntry(s) != null)
            return LocalizationSettings.StringDatabase.GetLocalizedString("TranslationTable", s);
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

    public void StopEffect()
    {
        effectsPlayer.Stop();
    }

    private IEnumerator ExecuteTriggers()
    {
        IsPlayerActive = false;
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
                            trig.displayDialogueSpeakerColor,
                            trig.displayDialogueContentColor,
                            trig.displayDialogueSub,
                            trig.displayDialogueFlash,
                            trig.displayDialogueSkippable,
                            trig.displayDialogueFlashLength
                        )
                    );
                    break;

                case TriggerType.ChangeScreen:
                    yield return StartCoroutine(
                        ChangeScreen(
                            trig.changeScreenStartColor,
                            trig.changeScreenEndColor,
                            trig.changeScreenLength,
                            trig.changeScreenSub,
                            trig.changeScreenFlash
                        )
                    );
                    break;

                case TriggerType.Wait:
                    yield return new WaitForSeconds(trig.waitLength);
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
                    if (trig.manageInventoryAddItem)
                        AddItem(trig.manageInventoryItem);
                    else
                        RemoveItem(trig.manageInventoryItem);
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

                        case TriggerType.Custom:
                            break;

                        default:
                            Debug.LogError("Trigger Not Found: " + trig.triggerType);
                            break;
                        }
        }

        IsPlayerActive = true;
        IsExecutingTriggers = false;
    }

    private IEnumerator DisplayDialogue(string speaker, string content, Color speakerColor, Color contentColor, bool sub, bool flash, bool skippable, float flashLength)
    {
        effectsPlayer.clip = writingEffect;
        effectsPlayer.Play();

        speaker = Translate(speaker);
        content = Translate(content);

        if (flash) IsPlayerActive = true;

        if (sub)
        {
            subdialogueSpeaker.text = speaker;
            subdialogueContent.text = "";
            subdialogueSpeaker.color = speakerColor;
            subdialogueContent.color = contentColor;
            subdialogueScreen.SetActive(true);
        }
        else
        {
            dialogueSpeaker.text = speaker;
            dialogueContent.text = "";
            dialogueSpeaker.color = speakerColor;
            dialogueContent.color = contentColor;
            dialogueScreen.SetActive(true);
        }

        int idx = 0;
        float t = 0, gap = displayGap[data.language];

        yield return new WaitForSeconds(0.05f);
        while (idx < content.Length)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                if (sub) subdialogueContent.text += content[idx];
                else dialogueContent.text += content[idx];
                idx++;
            }
            if ((skipInput && skippable))
            {
                if (sub) subdialogueContent.text = content;
                else dialogueContent.text = content;
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.05f);
        effectsPlayer.Stop();

        if (flash) yield return new WaitForSeconds(flashLength);
        else yield return new WaitUntil(() => skipInput && !IsPaused);

        if (sub) subdialogueScreen.SetActive(false);
        else dialogueScreen.SetActive(false);

        if (flash) IsPlayerActive = false;
    }

    private IEnumerator ChangeScreen(Color startColor, Color endColor, float length, bool sub, bool flash)
    {
        if (flash) IsPlayerActive = true;

        float t = 0;
        if (sub) subscreen.color = startColor;
        else screen.color = startColor;
        while (t < length)
        {
            yield return null;
            t += Time.deltaTime;
            if (sub) subscreen.color = Color.Lerp(startColor, endColor, t / length);
            else screen.color = Color.Lerp(startColor, endColor, t / length);
        }
        if (sub) subscreen.color = endColor;
        else screen.color = endColor;

        if (flash) IsPlayerActive = false;
    }

    private IEnumerator DisplayCanvas(GameObject canvas, AudioClip effect, bool flash, float flashLength)
    {
        if (!flash) CanPause = false;

        if (flash) IsPlayerActive = true;

        canvas.SetActive(true);
        PlayEffect(effect);

        if (flash)
            yield return new WaitForSeconds(flashLength);
        else
            yield return new WaitUntil(() => returnInput);

        canvas.SetActive(false);
        PlayEffect(effect);

        if (flash) IsPlayerActive = false;

        if (!flash) CanPause = true;
    }

    private IEnumerator LoadScene(string scene, float length, bool save)
    {
        yield return StartCoroutine(ChangeScreen(Color.clear, Color.black, length, false, false));
        SceneManager.LoadScene(scene);
    }

    private IEnumerator LoadMainMenu()
    {
        CanPause = false;
        pausedFrontScreen.raycastTarget = true;
        pausedFrontScreen.color = Color.clear;

        float t = 0f;
        while (t < 2f)
        {
            yield return null;
            t += Time.unscaledDeltaTime;
            pausedFrontScreen.color = Color.Lerp(Color.clear, Color.black, t / 2f);
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuName);
    }

    private IEnumerator DisplayEnding(string title, string description)
    {
        IsAtEnding = true;

        effectsPlayer.clip = writingEffect;
        effectsPlayer.Play();

        title = Translate(title);
        description = Translate(description);
        endingTitle.text = "";
        endingDescription.text = "";
        endingReturnMenuButton.SetActive(false);
        endingScreen.SetActive(true);
        screen.color = Color.clear;

        float t = 0, gap = displayGap[data.language];
        int idx = 0;
        while (idx < description.Length)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                endingDescription.text += description[idx];
                idx++;
            }
            if (skipInput)
            {
                endingDescription.text = description;
                break;
            }
            yield return null;
        }
        endingDescription.text = description;

        yield return new WaitForSeconds(0.05f);
        effectsPlayer.Stop();

        yield return new WaitUntil(() => skipInput);
        effectsPlayer.Play();

        t = 0;
        gap /= 10f;
        idx = description.Length;
        while (idx >= 0)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                endingDescription.text = endingDescription.text.Substring(0, idx);
                idx--;
            }
            if (skipInput)
            {
                endingDescription.text = "";
                break;
            }
            yield return null;
        }
        endingDescription.text = "";
        effectsPlayer.Stop();

        yield return new WaitForSeconds(1f);
        endingTitle.text = title;
        effectsPlayer.clip = endingEffect;
        effectsPlayer.Play();

        yield return new WaitForSeconds(1f);
        endingReturnMenuButton.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}