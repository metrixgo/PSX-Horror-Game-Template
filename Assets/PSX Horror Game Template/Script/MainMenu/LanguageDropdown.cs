using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    private Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }

    private void OnEnable()
    {
        dropdown.value = Lang2Idx(MainManager.instance.data.language);
    }

    public void ChangeLanguage(int index)
    {
        MainManager.instance.data.language = idx2Lang(index);
        MainManager.instance.SaveData();
    }

    private string idx2Lang(int idx)
    {
        if (idx == 0) return "English";
        else if (idx == 1) return "Chinese";
        else return "English";
    }

    private int Lang2Idx(string lang)
    {
        if (lang == "English") return 0;
        else if (lang == "Chinese") return 1;
        else return 0;
    }

}
