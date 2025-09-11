using UnityEngine;

public class Weight : MonoBehaviour
{
    public int index;
    public bool isSinful;
    public bool isMoral;

    [HideInInspector]
    public WheightManager manager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            //Debug.Log($"Exited: {collision.collider.name}");
            manager.ResetWeight(index);
        }
    }
}
