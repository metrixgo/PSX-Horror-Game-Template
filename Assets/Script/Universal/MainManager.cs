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

    public GameState gameState { get; private set; } = GameState.Executing;

    private List<Trigger> triggers = new List<Trigger>();
    private List<string> tasks = new List<string>();

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueScreen;
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    [SerializeField] private TextMeshProUGUI dialogueContent;

    [Header("Screen")]
    [SerializeField] private Image screen;
    [SerializeField] private Image subScreen;

    [Header("Pause")]
    [SerializeField] private GameObject pausedScreen;
    [SerializeField] private GameObject returnMenuButton;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private TextMeshProUGUI subPrompt;

    private void Awake()
    {
        instance = this;
    }

    public void SetPrompt(string s)
    {
        prompt.text = s;
    }

}
