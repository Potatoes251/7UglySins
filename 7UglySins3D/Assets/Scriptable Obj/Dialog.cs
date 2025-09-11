using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable Objects/Dialog")]
public class Dialog : ScriptableObject
{
    public string dialogName;

    public TextBox[] boxes;

    public float delay = 0.05f;

    public void DisplayDialog(int index = 0)
    {
        if (index < boxes.Length)
        {
            EventManager.TriggerEvent(GameEvent.StartText, new Dictionary<string, object> { });
            TextManager.instance.DisplayTexts(boxes[index], index, this);
        }
        else
        {
            TextManager.instance.StopDialogs();
        }
    }
}
