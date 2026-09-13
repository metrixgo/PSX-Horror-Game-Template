using TMPro;
using UnityEngine;

public class TranslateText : MonoBehaviour
{
    private TMP_Text txt;

    private void Start()
    {
        txt = GetComponent<TMP_Text>();
        txt.text = MainManager.instance.Translate(txt.text);
    }
}