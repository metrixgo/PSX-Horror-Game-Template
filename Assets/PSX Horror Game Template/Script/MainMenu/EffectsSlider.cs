using UnityEngine;
using UnityEngine.UI;

public class EffectsSlider : MonoBehaviour
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
        slider.value = PlayerPrefs.GetFloat("Effects", 80.0f);
        Save(slider.value);
    }

    public void Save(float n)
    {
        ad.volume = n / 100.0f;
        PlayerPrefs.SetFloat("Effects", n);
        PlayerPrefs.Save();
    }
}