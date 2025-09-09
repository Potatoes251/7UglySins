using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor.Experimental.GraphView;

public class TextManager : MonoBehaviour
{
    public Dialog[] dialogs;
    public TMP_Text uiText;

    public InputAction actionX;
    public InputAction actionY;

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
        StartDialogs("Test");
    }

    private void OnEnable()
    {
        actionX.performed += OnXPressed;
        actionY.performed += OnYPressed;

        actionX.Enable();
        actionY.Enable();
    }

    private void OnDisable()
    {
        actionX.Disable();
        actionY.Disable();
        actionX.performed -= OnXPressed;
        actionY.performed -= OnYPressed;
    }

    private void OnXPressed(InputAction.CallbackContext context)
    {
        StartDialogs("Test");
    }

    private void OnYPressed(InputAction.CallbackContext context)
    {
        StopDialogs();
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
