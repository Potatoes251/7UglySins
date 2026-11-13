using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startButton;

    [SerializeField]
    private GameObject image;

    public void ClickedStart()  
    {
        string message = "fuck";
        //Debug.Log("clicked");
        EventManager.TriggerEvent(GameEvent.ClickedStart, new Dictionary<string, object> { { "message", message } });
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }
    }

}
