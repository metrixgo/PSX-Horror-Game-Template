using UnityEngine;

public enum TriggerType
{
    DisplayDialogue,
    ChangeScreen,
    Wait,
}

[System.Serializable]
public class Trigger
{
    [SerializeField] private TriggerType type;
    public TriggerType Type => type;

    [SerializeField] private string dialogueSpeaker;
    [SerializeField] private string dialogueContent;
    [SerializeField] private bool dialogueSkippable;
    [SerializeField] private bool dialogueFlash;
    public string DialogueSpeaker => dialogueSpeaker;
    public string DialogueContent => dialogueContent;
    public bool DialogueSkippable => dialogueSkippable;
    public bool DialogueFlash => dialogueFlash;

    [SerializeField] private Color changeScreenStartColor;
    [SerializeField] private Color changeScreenEndColor;
    [SerializeField] private float changeScreenLength;
    public Color ChangeScreenStartColor => changeScreenStartColor;
    public Color ChangeScreenEndColor => changeScreenEndColor;
    public float ChangeScreenLength => changeScreenLength;

    [SerializeField] private float waitLength;
    [SerializeField] private bool waitFlash;
    public float WaitLength => waitLength;
    public bool WaitFlash => waitFlash;
}