using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour
{
    [SerializeField]
    private GameObject spawn;


    private void OnEnable()
    {
        EventManager.StartListening(GameEvent.EndText, ResetPosition);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("StampArea"))
        {
            Debug.Log("stamping");
            EventManager.TriggerEvent(GameEvent.GoNext, new Dictionary<string, object> { });
        }
    }

    private void ResetPosition(Dictionary<string, object> data)
    {
        transform.position = spawn.transform.position;
    }
}
