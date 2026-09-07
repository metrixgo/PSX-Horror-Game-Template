using System.Collections.Generic;
using UnityEngine;

public class TriggerSequences : MonoBehaviour
{
    [SerializeField] private List<Trigger> triggers;

    public void AddTriggers()
    {
        foreach (Trigger trigger in triggers)
        {
            MainManager.instance.AddTrigger(trigger);
        }
    }
}