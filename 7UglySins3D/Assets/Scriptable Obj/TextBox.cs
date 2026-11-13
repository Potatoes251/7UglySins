using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

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

    public IEnumerator DisplayText(TMP_Text uiText, Dialog dialog, AudioClip clip, CharID id, Image image, int index = 0)
    {
        var payload = new Dictionary<string, object> {
    { "clip", clip },
    { "fade", 1.2f },               // seconds (optional)
    { "volume", 0.9f },             // optional
    { "loop", true }                // optional, default true
};
        image.gameObject.SetActive(true);
        EventManager.TriggerEvent(GameEvent.PlaySfx, payload);
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
        EventManager.TriggerEvent(GameEvent.StopAllSfx, new Dictionary<string, object> { });
        if (dialog != null)
        {
            yield return new WaitUntil(() => continu );
            image.gameObject.SetActive(false);
            continu = false;
            dialog.DisplayDialog(id, ++index);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }

    private void OnInput(InputAction.CallbackContext context)
    {
        continu = true;
    }
}
