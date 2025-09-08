using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    public TextBox text;
    public TMP_Text uiText;

    public 

    void Start()
    {
        StartCoroutine(text.DisplayText(uiText));
    }

    void Update()
    {
        
    }
}
