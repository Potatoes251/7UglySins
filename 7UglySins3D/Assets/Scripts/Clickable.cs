using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Clickable : MonoBehaviour
{
    public GameObject objectToToggle;

    [SerializeField] 
    private InputAction clickAction;

    private void OnEnable()
    {
        clickAction.Enable();
        clickAction.performed += OnClick;
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClick;
        clickAction.Disable();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit)) // 3D version
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                objectToToggle.SetActive(!objectToToggle.activeSelf);
            }
        }
    }
}
