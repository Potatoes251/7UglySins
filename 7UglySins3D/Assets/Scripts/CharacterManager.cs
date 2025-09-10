using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    public Character[] character;

    public GameObject clickableID;
    public InitID id;

    private int currentChar = 0;

    private bool isCharacterPresent = false;

    [SerializeField]
    private WheightManager spawner = null;

    [SerializeField]
    private Transform spawnPoint;

    private void OnEnable()
    {
        EventManager.StartListening(GameEvent.NewCharacter, SwitchCharacter);
        EventManager.StartListening(GameEvent.Sentencing, SwitchCharacter);
    }

    public void SwitchCharacter(Dictionary<string, object> data)
    {
        if (isCharacterPresent)
        {
            Debug.Log("goToHell :" + (bool)data["goToHell"]);
            character[currentChar].Exits((bool)data["goToHell"]);
            ++currentChar;

            if (currentChar >= character.Length)
            {
                currentChar = 0;
                EventManager.TriggerEvent(GameEvent.LastCharacter, new Dictionary<string, object> { });
            }
        }
        else
        {
            character[currentChar].Enters(spawnPoint, spawner);
        }
        isCharacterPresent = !isCharacterPresent;

        if (clickableID != null)
        {
            clickableID.SetActive(isCharacterPresent);
            id.id = character[currentChar].id;
        }
    }
}
