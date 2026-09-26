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

    #if UNITY_EDITOR
        [ContextMenu("Add Default Trigger")]
        private void AddFreshTrigger()
        {
            UnityEditor.Undo.RecordObject(this, "Add Default Trigger");
            triggers.Add(new Trigger());
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif

    private void OnTriggerEnter(Collider other)
    {
        if (isPhysical && (!playerOnly || other.CompareTag("Player"))) AddTriggers();
    }

    public void AddTriggers()
    {
        foreach (Trigger trigger in triggers)
            MainManager.instance.AddTrigger(trigger);

        if (selfDestructs) Destroy(gameObject);
    }
}