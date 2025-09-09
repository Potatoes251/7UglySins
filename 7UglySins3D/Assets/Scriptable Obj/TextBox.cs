using TMPro;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "TextDisplay", menuName = "Scriptable Objects/TextDisplay")]
public class TextBox : ScriptableObject
{
    [TextArea(5, 10)]
    public string textToDisplay;

    public float delay = 0.05f;

    private string currentText = string.Empty;

    public IEnumerator DisplayText(TMP_Text uiText, Dialog dialog = null, int index = 0)
    {
        if (currentText != string.Empty) currentText = string.Empty;

        for (int i = 0; i < textToDisplay.Length; i++)
        {
            currentText += textToDisplay[i];
            uiText.text = currentText;
            yield return new WaitForSeconds(delay);
        }

        if (dialog != null)
        {
            yield return new WaitForSeconds(dialog.delay);
            dialog.DisplayDialog(++index);
        }
    }
}
