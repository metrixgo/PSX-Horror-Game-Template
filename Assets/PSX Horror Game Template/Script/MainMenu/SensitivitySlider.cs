using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        slider.value = MainManager.instance.data.sensitivity;
    }

    public void Save(float n)
    {
        MainManager.instance.data.sensitivity = n;
        MainManager.instance.SaveData();
    }
}