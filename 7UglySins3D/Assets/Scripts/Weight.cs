using UnityEngine;

public class Weight : MonoBehaviour
{
    public int index;
    public bool isSinful;
    public bool isMoral;

    [HideInInspector]
    public WheightManager manager;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayArea"))
        {
            Debug.Log($"Exited: {other.name}");
            //manager.ResetWeight(index);
        }
    }
}
