using System.Collections.Generic;
using UnityEngine;

public class TriggerSequences : MonoBehaviour
{
    [SerializeField] private List<Trigger> triggers;

    public void ExecuteTrigger()
    {
        foreach (Trigger trig in triggers)
        {
            switch (trig.Type)
            {
                case TriggerType.Dialogue:
                    break;
                case TriggerType.ChangeScreen:
                    break;
                case TriggerType.Wait:
                    break;
                default:
                    Debug.LogWarning("Unimplemented Trigger Type: " + trig.Type);
                    break;
            }
        }
    }
}