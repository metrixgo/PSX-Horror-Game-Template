using System.Collections.Generic;
using UnityEngine;

public class TriggerSequences : MonoBehaviour
{
    [SerializeField] private bool isPhysical = false;
    [SerializeField] private bool selfDestructs = false;
    [SerializeField] private List<Trigger> triggers;

    private void OnTriggerEnter(Collider other)
    {
        if (isPhysical) AddTriggers();
    }

    public void AddTriggers()
    {
        foreach (Trigger trigger in triggers)
        {
            MainManager.instance.AddTrigger(trigger);
        }

        if (selfDestructs) Destroy(gameObject);
    }
}