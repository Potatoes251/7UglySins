using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    public Character[] character;

    public GameObject clickableID;
    public InitID id;

    public InputAction action;

    private int currentChar = 0;
    private int prevChar = 0;

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
        if (clickableID != null)
        {
            clickableID.SetActive(true);
            id.id = character[currentChar].id;
        }

        character[prevChar].Exits();
        character[currentChar].Enters();

        prevChar = currentChar;
        ++currentChar;

        if (currentChar >= character.Length)
        { 
            currentChar = 0;
        }
    }
}
