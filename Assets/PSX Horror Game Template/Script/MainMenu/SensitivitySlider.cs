using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
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
        if (MainManager.instance != null)
            Get();
    }

    public void Get()
    {
        slider.SetValueWithoutNotify(MainManager.instance.data.sensitivity);
    }

    public void Save(float n)
    {
        MainManager.instance.data.sensitivity = n;
        MainManager.instance.SaveData();
    }
}