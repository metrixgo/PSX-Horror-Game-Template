using UnityEngine;

public enum TriggerType
{
    DisplayDialogue,
    ChangeScreen,
    Wait,
    DisplayPrompt,
    ManageTasks,
    ManageInventory,
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
    public TriggerType triggerType;

    public string displayDialogueSpeaker;
    public string displayDialogueContent;
    public Color displayDialogueSpeakerColor = Color.white;
    public Color displayDialogueContentColor = Color.white;
    public bool displayDialogueSub = true;
    public bool displayDialogueFlash;
    public bool displayDialogueSkippable = true;
    public float displayDialogueFlashLength;

    public Color changeScreenStartColor = Color.white;
    public Color changeScreenEndColor = Color.white;
    public float changeScreenLength = 1.5f;
    public bool changeScreenSub = true;
    public bool changeScreenFlash;

    public float waitLength = 1.5f;

    public string displayPromptPrompt;
    public Color displayPromptColor = Color.white;
    public bool displayPromptSub = true;
    public bool displayPromptFlash = true;

    public ManageTasksType manageTasksType;
    public string manageTasksTask;

    public string manageInventoryItem;
    public bool manageInventoryAddItem;

    public PlayerCanDoType playerCanDoType;
    public bool playerCanDoCanDo;

    public MovePlayerType movePlayerType;
    public Vector3 movePlayerVector;

    public Transform jumpscareAtObject;
    public float jumpscareAtLength = 1.5f;
    public AudioClip jumpscareAtEffect;

    public GameObject displayCanvasCanvas;
    public AudioClip displayCanvasEffect;
    public bool displayCanvasFlash;
    public float displayCanvasFlashLength;

    public AudioClip playSoundSound;
    public bool playSoundLocal = true;
    public bool playSoundIsEffect = true;
    public AudioSource playSoundSource;

    public GameObject setObjectObject;
    public bool setObjectSetActive = true;

    public string loadSceneScene;
    public float loadSceneLength = 1.5f;
    public bool loadSceneSave = true;

    public string displayEndingTitle;
    public string displayEndingDescription;
}