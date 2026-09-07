using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Normal,
    Executing,
    Paused,
}

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    public GameState gameState { get; private set; } = GameState.Normal;

    [Header("Sounds")]
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource effectsPlayer;

    [Header("Pause")]
    [SerializeField] private GameObject pausedScreen;
    [SerializeField] private GameObject returnMenuButton;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueScreen;
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    [SerializeField] private TextMeshProUGUI dialogueContent;
    [SerializeField] private GameObject subdialogueScreen;
    [SerializeField] private TextMeshProUGUI subdialogueContent;

    [Header("Screen")]
    [SerializeField] private Image screen;
    [SerializeField] private Image subscreen;

    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private TextMeshProUGUI subprompt;

    private float sensitivity;
    private float musicVolume;
    private float effectsVolume;
    private string savedScene;
    private string language;

    private AudioClip writingEffect;

    private List<Trigger> triggers = new List<Trigger>();
    private List<string> tasks = new List<string>();

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(triggers.Count > 0 && gameState == GameState.Normal)
        {
            StartCoroutine(ExecuteTriggers());
        }
    }

    private IEnumerator ExecuteTriggers()
    {
        while (triggers.Count > 0)
        {
            Trigger trig = triggers[0];

            switch (trig.Type)
            {
                case TriggerType.DisplayDialogue:
                    yield return StartCoroutine(DisplayDialogue(trig.DialogueSpeaker, trig.DialogueContent));
                    break;
                default:
                    Debug.LogError("Trigger Not Found: " + trig.Type);
                    break;
            }
        }
    }

    private IEnumerator DisplayDialogue(string speaker, string content)
    {
        effectsPlayer.clip = writingEffect;
        effectsPlayer.Play();
        dialogueSpeaker.text = speaker;
        dialogueContent.text = "";
        dialogueScreen.SetActive(true);
        int idx = 0;
        float t = 0, gap = 0.02f;
        if (language == "Chinese") gap = 0.04f;
        yield return new WaitForSeconds(0.05f);
        while (idx < content.Length)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                dialogueContent.text += content[idx];
                idx++;
            }
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                dialogueContent.text = content;
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        effectsPlayer.Stop();
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return));
        dialogueScreen.SetActive(false);
    }

    public void AddTrigger(Trigger trigger)
    {
        triggers.Add(trigger);
    }

    public void SetPrompt(string s)
    {
        prompt.text = s;
    }

}
