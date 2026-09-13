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
        dropdown.SetValueWithoutNotify(lang2Idx(MainManager.instance.data.language));
    }

    public void ChangeLanguage(int index)
    {
        MainManager.instance.data.language = idx2Lang(index);
        MainManager.instance.SaveData();

        manager.RefreshLanguage();
    }

    private string idx2Lang(int idx)
    {
        if (idx == 0) return "English";
        else if (idx == 1) return "Chinese";
        else return "English";
    }

    private int lang2Idx(string lang)
    {
        if (lang == "English") return 0;
        else if (lang == "Chinese") return 1;
        else return 0;
    }

}
