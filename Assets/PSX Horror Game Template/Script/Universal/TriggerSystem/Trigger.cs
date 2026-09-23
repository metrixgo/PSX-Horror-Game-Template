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
    JumpscareAt,
    DisplayCanvas,
    PlaySound,
    SetObject,
    LoadScene,
    DisplayEnding,
    Custom,
}

public enum ManageTasksType
{
    AddTask,
    RemoveTask,
    ClearTasks,
}

public enum PlayerCanDoType
{
    Look,
    Move,
    Sprint,
    Jump,
    Crouch,
    Interact,
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

    [SerializeField] private string displayDialogueSpeaker;
    [SerializeField] private string displayDialogueContent;
    [SerializeField] private Color displayDialogueSpeakerColor;
    [SerializeField] private Color displayDialogueContentColor;
    [SerializeField] private bool displayDialogueSub;
    [SerializeField] private bool displayDialogueFlash;
    [SerializeField] private bool displayDialogueSkippable;
    [SerializeField] private float displayDialogueFlashLength;
    public string DisplayDialogueSpeaker => displayDialogueSpeaker;
    public string DisplayDialogueContent => displayDialogueContent;
    public Color DisplayDialogueSpeakerColor => displayDialogueSpeakerColor;
    public Color DisplayDialogueContentColor => displayDialogueContentColor;
    public bool DisplayDialogueSub => displayDialogueSub;
    public bool DisplayDialogueFlash => displayDialogueFlash;
    public bool DisplayDialogueSkippable => displayDialogueSkippable;
    public float DisplayDialogueFlashLength => displayDialogueFlashLength;

    [SerializeField] private Color changeScreenStartColor;
    [SerializeField] private Color changeScreenEndColor;
    [SerializeField] private float changeScreenLength;
    [SerializeField] private bool changeScreenSub;
    [SerializeField] private bool changeScreenFlash;
    public Color ChangeScreenStartColor => changeScreenStartColor;
    public Color ChangeScreenEndColor => changeScreenEndColor;
    public float ChangeScreenLength => changeScreenLength;
    public bool ChangeScreenSub => changeScreenSub;
    public bool ChangeScreenFlash => changeScreenFlash;

    [SerializeField] private float waitLength;
    public float WaitLength => waitLength;

    [SerializeField] private string displayPromptPrompt;
    [SerializeField] private Color displayPromptColor;
    [SerializeField] private bool displayPromptSub;
    [SerializeField] private bool displayPromptFlash;
    public string DisplayPromptPrompt => displayPromptPrompt;
    public Color DisplayPromptColor => displayPromptColor;
    public bool DisplayPromptSub => displayPromptSub;
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

    [SerializeField] private Transform jumpscareAtObject;
    [SerializeField] private float jumpscareAtLength;
    [SerializeField] private AudioClip jumpscareAtEffect;
    public Transform JumpscareAtObject => jumpscareAtObject;
    public float JumpscareAtLength => jumpscareAtLength;
    public AudioClip JumpscareAtEffect => jumpscareAtEffect;

    [SerializeField] private GameObject displayCanvasCanvas;
    [SerializeField] private AudioClip displayCanvasEffect;
    [SerializeField] private bool displayCanvasFlash;
    [SerializeField] private float displayCanvasFlashLength;
    public GameObject DisplayCanvasCanvas => displayCanvasCanvas;
    public AudioClip DisplayCanvasEffect => displayCanvasEffect;
    public bool DisplayCanvasFlash => displayCanvasFlash;
    public float DisplayCanvasFlashLength => displayCanvasFlashLength;

    [SerializeField] private AudioClip playSoundSound;
    [SerializeField] private bool playSoundLocal;
    [SerializeField] private bool playSoundIsEffect;
    [SerializeField] private AudioSource playSoundSource;
    public AudioClip PlaySoundSound => playSoundSound;
    public bool PlaySoundLocal => playSoundLocal;
    public bool PlaySoundIsEffect => playSoundIsEffect;
    public AudioSource PlaySoundSource => playSoundSource;

    [SerializeField] private GameObject setObjectObject;
    [SerializeField] private bool setObjectSetActive;
    public GameObject SetObjectObject => setObjectObject;
    public bool SetObjectSetActive => setObjectSetActive;

    [SerializeField] private string loadSceneScene;
    [SerializeField] private float loadSceneLength;
    [SerializeField] private bool loadSceneSave;
    public string LoadSceneScene => loadSceneScene;
    public float LoadSceneLength => loadSceneLength;
    public bool LoadSceneSave => loadSceneSave;

    [SerializeField] private string displayEndingTitle;
    [SerializeField] private string displayEndingDescription;
    public string DisplayEndingTitle => displayEndingTitle;
    public string DisplayEndingDescription => displayEndingDescription;
}