using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable Objects/Dialog")]
public class Dialog : ScriptableObject
{
    public string dialogName;

    public TextBox[] boxes;

    public float delay = 0.05f;

    public void DisplayDialog(CharID id = null, int index = 0)
    {
        //Debug.Log("display dialog");
        if (index < boxes.Length)
        {
            //Debug.Log("inside display dialog");
            EventManager.TriggerEvent(GameEvent.StartText, new Dictionary<string, object> { });
            TextManager.instance.DisplayTexts(boxes[index], id, index, this);
        }
        else
        {
            TextManager.instance.StopDialogs();
        }
    }

    public void DisplayDialog(bool message, CharID id, int index = 0)
    {
        if (index < boxes.Length)
        {
            EventManager.TriggerEvent(GameEvent.StartText, new Dictionary<string, object> { });
            EventManager.TriggerEvent(GameEvent.NewCharacter, new Dictionary<string, object> { });
            TextManager.instance.DisplayTexts(boxes[index], id, index, this);
        }
        else
        {
            TextManager.instance.StopDialogs();
        }
    }
}
