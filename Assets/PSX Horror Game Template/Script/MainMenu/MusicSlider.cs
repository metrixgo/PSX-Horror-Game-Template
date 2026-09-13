using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    [SerializeField] private AudioSource musicPlayer;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = MainManager.instance.data.musicVolume;
        Save(slider.value);
    }

    public void Save(float n)
    {
        musicPlayer.volume = n;
        MainManager.instance.data.musicVolume = n;
        MainManager.instance.SaveData();
    }
}