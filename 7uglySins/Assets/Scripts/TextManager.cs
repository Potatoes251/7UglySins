using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor.Experimental.GraphView;

public class TextManager : MonoBehaviour
{
    public Dialog[] dialogs;
    public TMP_Text uiText;

    public InputAction actionX;


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
        Dialog Dia = FindDialog("Test");
        Dia.DisplayDialog();
    }

    private void OnEnable()
    {
        actionX.performed += OnXPressed;

        actionX.Enable();
    }

    private void OnDisable()
    {
        actionX.Disable();
        actionX.performed -= OnXPressed;
    }

    public void OnXPressed(InputAction.CallbackContext context)
    {
        Debug.Log("here");
        Dialog Dia = FindDialog("Test");
        Dia.DisplayDialog();
    }


    public void DisplayTexts(TextBox text, int index = 0, Dialog dialog = null)
    {
        StartCoroutine(text.DisplayText(uiText, dialog, index));
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
        return null;
    }
}
