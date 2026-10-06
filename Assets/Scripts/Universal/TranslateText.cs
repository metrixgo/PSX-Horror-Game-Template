using TMPro;
using UnityEngine;

public class TranslateText : MonoBehaviour
{
    private TMP_Text txt;

    private void Awake()
    {
        txt = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Translate();
    }

    private void OnEnable()
    {
        if(MainManager.Instance != null)
            Translate();
    }

    public void Translate()
    {
        txt.text = MainManager.Instance.Translate(gameObject.name);
    }
}