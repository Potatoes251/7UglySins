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

    private bool isCharacterPresent = false;

    [SerializeField]
    private WheightManager spawner = null;

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
        if (isCharacterPresent)
        {
            character[currentChar].Exits();
            ++currentChar;

            if (currentChar >= character.Length)
            {
                currentChar = 0;
            }
        }
        else
        {
            character[currentChar].Enters(spawner);
        }
        isCharacterPresent = !isCharacterPresent;

        if (clickableID != null)
        {
            clickableID.SetActive(isCharacterPresent);
            id.id = character[currentChar].id;
        }
    }
}
