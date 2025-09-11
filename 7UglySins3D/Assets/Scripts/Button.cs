using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Button : MonoBehaviour
{
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
        Debug.Log("Clicking");
        if (Physics.Raycast(ray, out RaycastHit hit)) // 3D version
        {
            Debug.Log(hit.collider);
            if (hit.collider.CompareTag("Button"))
            {
                Debug.Log("tag");
                EventManager.TriggerEvent(GameEvent.GoNext, new Dictionary<string, object> { });
            }
        }
    }
}
