using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    public Dialog[] dialogs;
    public TMP_Text uiText;

    static public TextManager instance;

    private bool isDialogPlaying = false;
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
        EventManager.StartListening(GameEvent.ClickedStart, Starting);
    }

    private void Starting(Dictionary<string, object> data)
    {
        StartDialogs("Boss");
    }

    private void Ending(Dictionary<string, object> data)
    {
        //Debug.Log("Ending is : " + (string)data["Ending"]);
        StartDialogs((string)data["Ending"]);
    }

    public void DisplayTexts(TextBox text, int index = 0, Dialog dialog = null)
    {
        //Debug.Log(index);
        uiText.gameObject.SetActive(true);
        StartCoroutine(text.DisplayText(uiText, dialog, index));
    }

    public void StartDialogs(string name)
    {
        if (isDialogPlaying)
        {
            StopDialogs();
        }
        isDialogPlaying = true;
        Dialog Dia = FindDialog(name);
        if (Dia != null)
        {
            Dia.DisplayDialog();
            EventManager.TriggerEvent(GameEvent.StartText, new Dictionary<string, object> { });
        }
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
        //Debug.Log("No dialogs named : " + name);
        return null;
    }

    public void StopDialogs()
    {
        isDialogPlaying = false;
        StopAllCoroutines();
        uiText.gameObject.SetActive(false);
        EventManager.TriggerEvent(GameEvent.EndText, new Dictionary<string, object> { });
    }
}
