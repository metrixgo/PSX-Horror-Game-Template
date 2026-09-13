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

    private void Start()
    {
        Get();
    }

    private void OnEnable()
    {
        if(MainManager.instance != null)
            Get();
    }

    public void Get()
    {
        slider.SetValueWithoutNotify(MainManager.instance.data.musicVolume);
    }

    public void Save(float n)
    {
        effectsPlayer.volume = n;
        MainManager.instance.data.effectsVolume = n;
        MainManager.instance.SaveData();
    }
}