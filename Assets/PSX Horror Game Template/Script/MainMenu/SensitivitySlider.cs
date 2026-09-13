using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = MainManager.instance.data.sensitivity;
        Save(slider.value);
    }

    public void Save(float n)
    {
        MainManager.instance.data.sensitivity = n;
        MainManager.instance.SaveData();
    }
}