using UnityEditor;
using UnityEngine;

public enum TriggerType
{
    DisplayDialogue,
    ChangeScreen,
    Wait,
    DisplayPrompt,
    ManageTasks,
    PlayerCanDo,
    MovePlayer,
}

public enum DisplayDialogueType
{
    Main,
    Sub,
    FlashMain,
    FlashSub,
}

public enum ChangeScreenType
{
    Main,
    Sub,
    FlashMain,
    FlashSub,
}

public enum ManageTasksType
{
    AddTask,
    RemoveTask,
    ClearAllTasks,
}

public enum PlayerCanDoType
{
    Look,
    Move,
    Run,
    Jump,
    Crouch,
}

public enum MovePlayerType
{
    Location,
    Direction,
}

[System.Serializable]
public class Trigger
{
    [SerializeField] private TriggerType triggerType;
    public TriggerType TriggerType => triggerType;

    [SerializeField] private DisplayDialogueType displayDialogueType;
    [SerializeField] private string displayDialogueSpeaker;
    [SerializeField] private string displayDialogueContent;
    [SerializeField] private bool displayDialogueSkippable;
    [SerializeField] private float displayDialogueFlashLength;
    public DisplayDialogueType DisplayDialogueType => displayDialogueType;
    public string DisplayDialogueSpeaker => displayDialogueSpeaker;
    public string DisplayDialogueContent => displayDialogueContent;
    public bool DisplayDialogueSkippable => displayDialogueSkippable;
    public float DisplayDialogueFlashLength => displayDialogueFlashLength;

    [SerializeField] private ChangeScreenType changeScreenType;
    [SerializeField] private Color changeScreenStartColor;
    [SerializeField] private Color changeScreenEndColor;
    [SerializeField] private float changeScreenLength;
    public ChangeScreenType ChangeScreenType => changeScreenType;
    public Color ChangeScreenStartColor => changeScreenStartColor;
    public Color ChangeScreenEndColor => changeScreenEndColor;
    public float ChangeScreenLength => changeScreenLength;

    [SerializeField] private float waitLength;
    public float WaitLength => waitLength;

    [SerializeField] private string displayPromptPrompt;
    [SerializeField] private bool displayPromptFlash;
    public string DisplayPromptPrompt => displayPromptPrompt;
    public bool DisplayPromptFlash => displayPromptFlash;

    [SerializeField] private ManageTasksType manageTasksType;
    [SerializeField] private string manageTasksTask;
    public ManageTasksType ManageTasksType => manageTasksType;
    public string ManageTasksTask => manageTasksTask;

    [SerializeField] private PlayerCanDoType playerCanDoType;
    [SerializeField] private bool playerCanDoCanDo;
    public PlayerCanDoType PlayerCanDoType => playerCanDoType;
    public bool PlayerCanDoCanDo => playerCanDoCanDo;

    [SerializeField] private MovePlayerType movePlayerType;
    [SerializeField] private Vector3 movePlayerVector;
    public MovePlayerType MovePlayerType => movePlayerType;
    public Vector3 MovePlayerVector => movePlayerVector;
}