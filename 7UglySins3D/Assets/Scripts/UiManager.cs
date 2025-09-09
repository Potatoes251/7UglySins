using UnityEngine;
using System.Collections.Generic;

public class UiManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
       
    }

    public void ClickedStart()
    {
        string message = "fuck";
        Debug.Log("clicked");
        EventManager.TriggerEvent(GameEvent.ClickedStart, new Dictionary<string, object> { { "message", message } });
    }

}
