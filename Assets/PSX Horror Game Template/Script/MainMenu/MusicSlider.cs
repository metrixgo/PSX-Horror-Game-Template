using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    [SerializeField] private AudioSource musicPlayer;
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
        musicPlayer.volume = n;
        MainManager.instance.data.musicVolume = n;
        MainManager.instance.SaveData();
    }
}