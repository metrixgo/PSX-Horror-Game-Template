using UnityEngine;

public enum TriggerType
{
    DisplayDialogue,
    ChangeScreen,
    Wait,
    DisplayPrompt,
    ClearTasks,
    AddTasks,
    RemoveTasks,
    PlayerCanLook,
    PlayerCanMove,
    PlayerCanRun,
    PlayerCanJump,
    PlayerCanCrouch,
}

[System.Serializable]
public class Trigger
{
    [SerializeField] private TriggerType triggerType;
    public TriggerType TriggerType => triggerType;

    [SerializeField] private string displayDialogueSpeaker;
    [SerializeField] private string displayDialogueContent;
    [SerializeField] private bool displayDialogueSub;
    [SerializeField] private bool displayDialogueSkippable;
    [SerializeField] private bool displayDialogueTimedQuit;
    [SerializeField] private float displayDialogueQuitLength;
    public string DisplayDialogueSpeaker => displayDialogueSpeaker;
    public string DisplayDialogueContent => displayDialogueContent;
    public bool DisplayDialogueSub => displayDialogueSub;
    public bool DisplayDialogueSkippable => displayDialogueSkippable;
    public bool DisplayDialogueTimedQuit => displayDialogueTimedQuit;
    public float DisplayDialogueQuitLength => displayDialogueQuitLength;

    [SerializeField] private Color changeScreenStartColor;
    [SerializeField] private Color changeScreenEndColor;
    [SerializeField] private float changeScreenLength;
    [SerializeField] private bool changeScreenSub;
    public Color ChangeScreenStartColor => changeScreenStartColor;
    public Color ChangeScreenEndColor => changeScreenEndColor;
    public float ChangeScreenLength => changeScreenLength;
    public bool ChangeScreenSub => changeScreenSub;

    [SerializeField] private float waitLength;
    public float WaitLength => waitLength;
}