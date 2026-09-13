using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    private Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }

    public void ChangeLanguage(int index)
    {
        MainManager.instance.data.language = dropdown.options[index].text;
        MainManager.instance.SaveData();
    }
}
