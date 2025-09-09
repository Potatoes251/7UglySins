using UnityEngine;
using UnityEngine.InputSystem;

public class IDdisplay : MonoBehaviour
{
    [SerializeField] private GameObject id;

    [SerializeField] private InputAction clickAction;

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
            if (hit.collider.CompareTag("id"))
            {
                id.SetActive(!id.activeSelf);
            }
        }
        
    }
}
