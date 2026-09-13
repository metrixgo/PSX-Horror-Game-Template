using UnityEngine;
using UnityEngine.UI;

public class EffectsSlider : MonoBehaviour
{
    [SerializeField] private AudioSource effectsPlayer;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        slider.value = MainManager.instance.data.effectsVolume;
    }

    public void Save(float n)
    {
        effectsPlayer.volume = n;
        MainManager.instance.data.effectsVolume = n;
        MainManager.instance.SaveData();
    }
}