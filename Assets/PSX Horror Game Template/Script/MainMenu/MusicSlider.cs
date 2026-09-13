using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    [SerializeField] private AudioSource ad;
    private Slider slider;

    private void OnEnable()
    {
        slider = GetComponent<Slider>();
        Set();
    }

    public void Set()
    {
        slider.value = PlayerPrefs.GetFloat("Music", 30.0f);
        Save(slider.value);
    }

    public void Save(float n)
    {
        ad.volume = n / 100.0f;
        PlayerPrefs.SetFloat("Music", n);
        PlayerPrefs.Save();
    }
}