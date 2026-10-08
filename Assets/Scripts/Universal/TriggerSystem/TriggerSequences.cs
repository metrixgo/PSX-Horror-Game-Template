using System.Collections.Generic;
using UnityEngine;

public class TriggerSequences : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private bool isPhysical = false;
    [SerializeField] private bool selfDestructs = false;
    [SerializeField] private bool playerOnly = true;

    [Header("Triggers")]
    [SerializeField] private List<Trigger> triggers = new List<Trigger>();

    private void OnTriggerEnter(Collider other)
    {
        if (isPhysical && (!playerOnly || other.CompareTag("Player")))
            AddTriggers();
    }

    public void AddTriggers()
    {
        if (MainManager.Instance.IsExecutingTriggers) return;

        foreach (Trigger trigger in triggers)
            MainManager.Instance.AddTrigger(trigger);

        if (selfDestructs) Destroy(gameObject);
    }
}