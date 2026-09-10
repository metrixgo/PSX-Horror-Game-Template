using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string prompt;
    [SerializeField] private UnityEvent onInteraction;

    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
        SetFocused(false);
    }

    public void Interact()
    {
        SetFocused(false);
        onInteraction.Invoke();
    }

    public void SetFocused(bool b)
    {
        if (outline) outline.enabled = b;
        if (b) MainManager.instance.SetPrompt(prompt, false);
        else MainManager.instance.SetPrompt("", false);
    }
}