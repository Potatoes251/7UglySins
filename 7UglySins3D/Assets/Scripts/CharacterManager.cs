using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    public Character[] character;

    public InputAction action;

    private void OnEnable()
    {
        action.performed += OnActionPressed;

        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
        action.performed -= OnActionPressed;
    }

    public void OnActionPressed(InputAction.CallbackContext context)
    {
        character[0].Enters();
    }
}
