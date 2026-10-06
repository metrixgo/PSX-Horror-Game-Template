using UnityEngine;

public class SetSound : MonoBehaviour
{
    [SerializeField] private bool isEffect = true;
    [SerializeField] private float multiplier = 1;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) Debug.LogError("Set Sound Script Cannot Find Audio Source On" + gameObject.name);
        else Set();
    }

    public void Set()
    {
        if (isEffect) audioSource.volume = MainManager.Instance.Data.effectsVolume * multiplier;
        else audioSource.volume = MainManager.Instance.Data.musicVolume * multiplier;
    }
}
