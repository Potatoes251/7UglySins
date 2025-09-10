using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

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
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }
    }

}
