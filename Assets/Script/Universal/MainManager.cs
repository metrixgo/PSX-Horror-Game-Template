using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI taskPrompt;

    private float sensitivity;
    private float musicVolume;
    private float effectsVolume;
    private string savedScene;
    private string language;

    private AudioClip writingEffect;

    private List<Trigger> triggers = new List<Trigger>();
    private List<string> tasks = new List<string>();

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
        if (!IsExecutingTriggers && triggers.Count > 0 && gameState == GameState.Normal)
        {
            IsExecutingTriggers = true;
            StartCoroutine(ExecuteTriggers());
        }
    }

    public void AddTrigger(Trigger trigger)
    {
        triggers.Add(trigger);
    }

    public void SetPrompt(string s)
    {
        prompt.text = Translate(s);
    }

    public string Translate(string s)
    {
        if (language == "English") return s;
        if (translations.ContainsKey(s)) return translations[s];
        return s;
    }

    private IEnumerator ExecuteTriggers()
    {
        while (triggers.Count > 0)
        {
            Trigger trig = triggers[0];
            triggers.RemoveAt(0);

            switch (trig.TriggerType)
            {
                case TriggerType.DisplayDialogue:
                    yield return StartCoroutine(
                        DisplayDialogue(
                            trig.DisplayDialogueType,
                            trig.DisplayDialogueSpeaker,
                            trig.DisplayDialogueContent,
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
                            trig.ChangeScreenSub
                        )
                    );
                    break;

                case TriggerType.Wait:
                    yield return new WaitForSeconds(trig.WaitLength);
                    break;



                default:
                    Debug.LogError("Trigger Not Found: " + trig.TriggerType);
                    break;
            }
        }

        IsExecutingTriggers = false;
    }

    private IEnumerator DisplayDialogue(DisplayDialogueType type, string speaker, string content, bool skippable, float flashLength)
    {
        effectsPlayer.clip = writingEffect;
        effectsPlayer.Play();

        bool sub = type == DisplayDialogueType.Sub || type == DisplayDialogueType.FlashSub;
        bool flash = type == DisplayDialogueType.FlashMain || type == DisplayDialogueType.FlashSub;
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
    }

    private IEnumerator ChangeScreen(Color startColor, Color endColor, float length, bool sub)
    {
        float t = 0;
        screen.color = startColor;
        while (t < length)
        {
            yield return null;
            t += Time.deltaTime;
            screen.color = Color.Lerp(startColor, endColor, t / length);
        }
        screen.color = endColor;
    }
}