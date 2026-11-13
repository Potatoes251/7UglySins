using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Dialog[] dialogs;
    public TMP_Text uiText;

    static public TextManager instance;

    [SerializeField]
    private AudioClip clip;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite defaultBack;

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

    public void DisplayTexts(TextBox text, CharID id, int index = 0, Dialog dialog = null)
    {
        Debug.Log(id);
        if (image != null && id != null && id.background != null)
        {
            Debug.Log(id.name);
            image.gameObject.SetActive(true);
            image.sprite = id.background;
        }
        else
        {
            image.gameObject.SetActive(true);
            image.sprite = defaultBack;
        }
        //Debug.Log(index);
        uiText.gameObject.SetActive(true);
        StartCoroutine(text.DisplayText(uiText, dialog, clip, id, image, index));
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
