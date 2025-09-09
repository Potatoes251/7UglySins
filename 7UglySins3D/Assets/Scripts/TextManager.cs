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
        Dialog Dia = FindDialog("Test");
        Dia.DisplayDialog();
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

    public void OnXPressed(InputAction.CallbackContext context)
    {
        Dialog Dia = FindDialog("Test");
        Dia.DisplayDialog();
    }

    public void OnYPressed(InputAction.CallbackContext context)
    {
        Dialog Dia = FindDialog("Test 2");
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
