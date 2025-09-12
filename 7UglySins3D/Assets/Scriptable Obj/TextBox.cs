using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "TextDisplay", menuName = "Scriptable Objects/TextDisplay")]
public class TextBox : ScriptableObject
{
    [TextArea(5, 10)]
    [SerializeField]
    private string textToDisplay;

    [SerializeField]
    private float speed = 0.05f;

    private string currentText = string.Empty;

    [SerializeField]
    private InputAction nextBox;

    private bool continu;

    private void OnEnable()
    {
        nextBox = new InputAction("NextBox", InputActionType.Button);
        nextBox.AddBinding("<Keyboard>/space");
        nextBox.AddBinding("<Keyboard>/enter");

        nextBox.performed += OnInput;
        nextBox.Enable();
    }

    private void OnDisable()
    {
        nextBox.performed -= OnInput;
        nextBox.Disable();
    }

    public IEnumerator DisplayText(TMP_Text uiText, Dialog dialog, int index = 0)
    {
        continu = false;
        if (currentText != string.Empty) currentText = string.Empty;

        if (textToDisplay == null || textToDisplay == string.Empty)
        {
            Debug.LogWarning("missing text in text box : " + name);
        }
        for (int i = 0; i < textToDisplay.Length; i++)
        {
            currentText += textToDisplay[i];
            uiText.text = currentText;
            yield return new WaitForSeconds(speed);
            if (continu)
            {
                break;
            }
        }
        uiText.text = textToDisplay;
        continu = false;
        if (dialog != null)
        {
            yield return new WaitUntil(() => continu );
            continu = false;
            dialog.DisplayDialog(++index);
        }
    }

    private void OnInput(InputAction.CallbackContext context)
    {
        continu = true;
    }
}
