using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [SerializeField]
    private GameObject blocker;

    private void OnEnable()
    {
        EventManager.StartListening(GameEvent.StartText, Block);
        EventManager.StartListening(GameEvent.EndText, UnBlock);
    }

    private void Block(Dictionary<string, object> data)
    {
        Debug.Log("Blocking");
        blocker.SetActive(true);
    }

    private void UnBlock(Dictionary<string, object> data)
    {
        Debug.Log("Blocking");
        blocker.SetActive(false);
    }
}
