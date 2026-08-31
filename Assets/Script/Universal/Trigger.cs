using UnityEngine;

public enum TriggerType
{
    Dialogue,
    ChangeScreen,
    Wait,
}

[System.Serializable]
public class Trigger
{
    [SerializeField] private TriggerType type;

    [SerializeField] private string dialogueSpeaker;
    [SerializeField] private string dialogueContent;
    [SerializeField] private bool dialogueSkippable;
    [SerializeField] private bool dialogueFlash;

    [SerializeField] private Color changeScreenStartColor;
    [SerializeField] private Color changeScreenEndColor;
    [SerializeField] private float changeScreenLength;

    [SerializeField] private float waitLength;
    [SerializeField] private bool waitFlash;

    public void ExecuteTrigger()
    {
        switch (type)
        {
            case TriggerType.Dialogue:
                break;
            case TriggerType.ChangeScreen:
                break;
            case TriggerType.Wait:
                break;
            default:
                Debug.LogWarning("Unimplemented Trigger Type: " + type);
                break;
        }
    }
}