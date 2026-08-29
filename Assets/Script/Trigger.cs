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

    [SerializeField] private string dialogueSpeaker;
    [SerializeField] private string dialogueContent;

    [SerializeField] private Color startScreenColor;
    [SerializeField] private Color endScreenColor;
    [SerializeField] private float changeScreenLength;

    [SerializeField] private float waitLength;

    public void ExecuteTrigger()
    {
        switch (type)
        {
            case TriggerType.DisplayDialogue:
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