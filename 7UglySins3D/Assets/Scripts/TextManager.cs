using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    public Dialog[] dialogs;
    public TMP_Text uiText;

    static public TextManager instance;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(GameEvent.Ending, Ending);
    }

    private void Ending(Dictionary<string, object> data)
    {
        Debug.Log("Ending is : " + (string)data["Ending"]);
        StartDialogs((string)data["Ending"]);
    }

    public void DisplayTexts(TextBox text, int index = 0, Dialog dialog = null)
    {
        uiText.gameObject.SetActive(true);
        StartCoroutine(text.DisplayText(uiText, dialog, index));
    }

    public void StartDialogs(string name)
    {
        StopDialogs();
        Dialog Dia = FindDialog(name);
        if (Dia != null) Dia.DisplayDialog();
    }

    public Dialog FindDialog(string name)
    {
        for (int i = 0; i < dialogs.Length; i++)
        {
            if (dialogs[i].dialogName == name)
            {
                return dialogs[i];
            }
        }
        Debug.Log("No dialogs named : " + name);
        return null;
    }

    public void StopDialogs()
    {
        StopAllCoroutines();
        uiText.gameObject.SetActive(false);
    }
}
