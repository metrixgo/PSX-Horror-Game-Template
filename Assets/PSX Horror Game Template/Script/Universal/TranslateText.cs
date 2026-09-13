using TMPro;
using UnityEngine;

public class TranslateText : MonoBehaviour
{
    private TextMeshProUGUI txt;
    private TextMeshPro txt2;

    private void OnEnable()
    {
        txt = GetComponent<TextMeshProUGUI>();
        if (txt == null) txt2 = GetComponent<TextMeshPro>();
        if (PlayerPrefs.GetString("Language", "English") == "Chinese")
        {
            string s = gameObject.name.ToLower();
            string r;
            if (s == "options") r = "ѡ��";
            else if (s == "start") r = "��ʼ";
            else if (s == "quit") r = "�˳�";
            else if (s == "we'll be there.") r = "���ǻ��ڡ�";
            else if (s == "language") r = "����";
            else if (s == "sensitivity") r = "������";
            else if (s == "music") r = "����";
            else if (s == "sfx") r = "��Ч";
            else if (s == "back") r = "����";
            else if (s == "paused") r = "��ͣ";
            else if (s == "main menu") r = "���˵�";
            else if (s == "esc to continue...") r = "��Esc����...";
            else if (s == "nowhere to run :)") r = "�޴����� :)";
            else if (s == "5 hours later...") r = "5 Сʱ��...";
            else if (s == "continue") r = "����";
            else if (s == "You should've went for the entrance...") r = "�㵱ʱӦ��ȥ��ڵ�...";
            else if (s == "...Except you didn't.") r = "...ֻ������û�С�";
            else if (s == "[esc] to exit") r = "[Esc] �˳�";
            else if (s == "run.") r = "�ܡ�";
            else if (s == "clear data") r = "�������";
            else if (s == "thank you for playing my game!") r = "��л�������ҵ���Ϸ��";
            else r = s;
            if (txt != null) txt.text = r;
            else txt2.text = r;
        }
        else
        {
            if (txt != null) txt.text = gameObject.name;
            else txt2.text = gameObject.name;
        }
    }
}