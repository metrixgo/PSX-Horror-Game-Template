using TMPro;
using UnityEngine;

public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] private MainMenuManager manager;

    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
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
        dropdown.SetValueWithoutNotify(MainManager.instance.data.language);
    }

    public void ChangeLanguage(int index)
    {
        MainManager.instance.data.language = index;
        MainManager.instance.SaveData();

        manager.ChangeLanguage(index);
    }
}
