using System.Collections.Generic;
using UnityEngine;

public class TriggerSequences : MonoBehaviour
{
    [SerializeField] private bool isPhysical = false;
    [SerializeField] private bool selfDestructs = false;
    [SerializeField] private bool playerOnly = true;
    [SerializeField] private List<Trigger> triggers = new List<Trigger>();

    private void OnTriggerEnter(Collider other)
    {
        if (isPhysical && (!playerOnly || other.CompareTag("Player"))) AddTriggers();
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