using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Normal,
    Paused,
}

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    public GameState gameState { get; private set; } = GameState.Normal;

    public bool IsPlayerActive { get; private set; } = true;
    public bool IsExecutingTriggers { get; private set; } = false;
    public bool IsPaused { get; private set; } = false;

    private bool CanPause = true;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Sounds")]
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource effectsPlayer;

    [Header("Pause")]
    [SerializeField] private GameObject pausedScreen;

    [Header("Ending")]
    [SerializeField] private GameObject endingScreen;
    [SerializeField] private Image endingFrontScreen;
    [SerializeField] private TextMeshProUGUI endingType;
    [SerializeField] private TextMeshProUGUI endingDescription;
    [SerializeField] private GameObject endingReturnMenuButton;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueScreen;
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    [SerializeField] private TextMeshProUGUI dialogueContent;
    [SerializeField] private GameObject subdialogueScreen;
    [SerializeField] private TextMeshProUGUI subdialogueSpeaker;
    [SerializeField] private TextMeshProUGUI subdialogueContent;
    private float englishGap = 0.02f;
    private float nonEnglishGap = 0.04f;

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

    private float sensitivity;
    private float musicVolume;
    private float effectsVolume;
    private string savedScene;
    private string language;

    private AudioClip writingEffect;

    private List<Trigger> triggers = new List<Trigger>();

    private Dictionary<string, string> translations = new Dictionary<string, string>()
    {
        { "Hello!", "ÄãºÃ£¡"},
    };

    private void Awake()
    {
        instance = this;

        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 10f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100f) / 100f;
        effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 100f) / 100f;
        savedScene = PlayerPrefs.GetString("SavedScene", "");
        language = PlayerPrefs.GetString("Language", "English");
    }

    private void Update()
    {
        if (!IsExecutingTriggers && triggers.Count > 0 && gameState == GameState.Normal) StartCoroutine(ExecuteTriggers());

        UpdatePrompts();

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

    public void AddTrigger(Trigger trigger)
    {
        triggers.Add(trigger);
    }

    public string Translate(string s)
    {
        if (language == "English") return s;
        if (translations.ContainsKey(s)) return translations[s];
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
        if (tasks.Contains(s)) return;
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

    public void PlayMusic(AudioClip music)
    {
        musicPlayer.clip = music;
        musicPlayer.Play();
    }

    public void PlayEffect(AudioClip effect)
    {
        effectsPlayer.clip = effect;
        effectsPlayer.Play();
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

            switch (trig.TriggerType)
            {
                case TriggerType.DisplayDialogue:
                    yield return StartCoroutine(
                        DisplayDialogue(
                            trig.DisplayDialogueSpeaker,
                            trig.DisplayDialogueContent,
                            trig.DisplayDialogueSub,
                            trig.DisplayPromptFlash,
                            trig.DisplayDialogueSkippable,
                            trig.DisplayDialogueFlashLength
                        )
                    );
                    break;

                case TriggerType.ChangeScreen:
                    yield return StartCoroutine(
                        ChangeScreen(
                            trig.ChangeScreenStartColor,
                            trig.ChangeScreenEndColor,
                            trig.ChangeScreenLength,
                            trig.ChangeScreenSub,
                            trig.ChangeScreenFlash
                        )
                    );
                    break;

                case TriggerType.Wait:
                    yield return new WaitForSeconds(trig.WaitLength);
                    break;

                case TriggerType.DisplayPrompt:
                    SetPrompt(trig.DisplayPromptPrompt, trig.DisplayPromptColor, trig.DisplayPromptSub, trig.DisplayPromptFlash);
                    break;

                case TriggerType.ManageTasks:
                    switch (trig.ManageTasksType)
                    {
                        case ManageTasksType.AddTask:
                            AddTask(trig.ManageTasksTask);
                            break;
                        case ManageTasksType.RemoveTask:
                            RemoveTask(trig.ManageTasksTask);
                            break;
                        case ManageTasksType.ClearTasks:
                            ClearTasks();
                            break;
                        default:
                            Debug.LogWarning("Unimplemented Manage Tasks Type: " + trig.ManageTasksType);
                            break;
                    }
                    break;

                case TriggerType.PlayerCanDo:
                    switch (trig.PlayerCanDoType)
                    {
                        case PlayerCanDoType.Look:
                            player.CanLook(trig.PlayerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Move:
                            player.CanMove(trig.PlayerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Run:
                            player.CanRun(trig.PlayerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Jump:
                            player.CanJump(trig.PlayerCanDoCanDo);
                            break;
                        case PlayerCanDoType.Crouch:
                            player.CanCrouch(trig.PlayerCanDoCanDo);
                            break;
                        default:
                            Debug.LogWarning("Unimplemented Player Can Do Type: " + trig.PlayerCanDoType);
                            break;
                    }
                    break;

                case TriggerType.MovePlayer:
                    switch (trig.MovePlayerType)
                    {
                        case MovePlayerType.Location:
                            player.SetPosition(trig.MovePlayerVector);
                            break;
                        case MovePlayerType.Direction:
                            player.Move(trig.MovePlayerVector);
                            break;
                        default:
                            Debug.LogWarning("Unimplemented Move Player Type: " + trig.MovePlayerType);
                            break;
                    }
                    break;

                case TriggerType.JumpscareAt:
                    player.LookAt(trig.JumpscareAtPosition, trig.JumpscareAtLength);
                    PlayEffect(trig.JumpscareAtEffect);
                    yield return new WaitForSeconds(trig.JumpscareAtLength);
                    break;

                case TriggerType.DisplayCanvas:
                    yield return StartCoroutine(
                        DisplayCanvas(
                            trig.DisplayCanvasCanvas,
                            trig.DisplayCanvasEffect,
                            trig.DisplayCanvasFlash,
                            trig.DisplayCanvasFlashLength
                        )
                    );
                    break;

                case TriggerType.PlaySound:
                    break;

                case TriggerType.SetObject:
                    break;

                case TriggerType.LoadScene:
                    break;

                case TriggerType.DisplayEnding:
                    break;

                case TriggerType.Custom:
                    break;

                default:
                    Debug.LogError("Trigger Not Found: " + trig.TriggerType);
                    break;
            }
        }

        IsPlayerActive = true;
        IsExecutingTriggers = false;
    }

    private IEnumerator DisplayDialogue(string speaker, string content, bool sub, bool flash, bool skippable, float flashLength)
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
            subdialogueScreen.SetActive(true);
        }
        else
        {
            dialogueSpeaker.text = speaker;
            dialogueContent.text = "";
            dialogueScreen.SetActive(true);
        }

        int idx = 0;
        float t = 0, gap = language == "English" ? englishGap : nonEnglishGap;

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
            if ((Input.GetMouseButtonDown(0)) && skippable)
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
        else yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
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
        CanPause = false;

        if (flash) IsPlayerActive = true;

        canvas.SetActive(true);

        if (flash) yield return new WaitForSeconds(flashLength);
        else yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        if (flash) IsPlayerActive = false;

        CanPause = true;
    }
}