using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
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
        Debug.Log("GameManagerEnabled");
        EventManager.StartListening(GameEvent.ClickedStart, TestEventGameStart);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameEvent.ClickedStart, TestEventGameStart);
    }

    void TestEventGameStart(Dictionary<string, object> data)
    {
        Debug.Log("GameManagerFunctionTriggered");
        Debug.Log( (string)data["message"]);
    }
}
