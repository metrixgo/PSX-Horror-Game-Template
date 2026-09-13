using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    private Slider slider;

    private void OnEnable()
    {
        slider = GetComponent<Slider>();
        Set();
    }

    public void Set()
    {
        slider.value = PlayerPrefs.GetFloat("Sensitivity", 10.0f);
        Save(slider.value);
    }

    public void Save(float n)
    {
        PlayerPrefs.SetFloat("Sensitivity", n);
        PlayerPrefs.Save();
    }
}