using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Weight : MonoBehaviour
{
    public int index;
    public bool isSinful;
    public bool isMoral;

    [HideInInspector]
    public WheightManager manager;

    [SerializeField]
    private List<AudioClip> clip;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            //Debug.Log($"Exited: {collision.collider.name}");
            manager.ResetWeight(index);
        }
        else if (collision.collider.CompareTag("table"))
        {
            var payload = new Dictionary<string, object> {
    { "clip", clip[0] },
    { "pos", collision.collider.ClosestPoint(transform.position) },           // world position
    { "spatial", 1f },              // 3D
    { "volume", 10f }
};
            EventManager.TriggerEvent(GameEvent.PlaySfx, payload);
        }
        else if (collision.collider.CompareTag("scale"))
        {
            var payload = new Dictionary<string, object> {
    { "clip", clip[1] },
     { "pos", collision.collider.ClosestPoint(transform.position) },           // world position
    { "spatial", 1f },              // 3D
    { "volume", 1f }
};
            EventManager.TriggerEvent(GameEvent.PlaySfx, payload);
        }
    }
}
