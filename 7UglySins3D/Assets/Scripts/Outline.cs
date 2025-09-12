using UnityEngine;

public class Outline : MonoBehaviour
{
    [SerializeField]
    private GameObject outline;

    private void OnMouseEnter()
    {
        outline.SetActive(true);
    }

    private void OnMouseExit()
    {
        outline.SetActive(false);
    }
}
