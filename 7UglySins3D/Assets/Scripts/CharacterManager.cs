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

    private bool characterWaiting = false;

    private void OnEnable()
    {
        EventManager.StartListening(GameEvent.NewCharacter, NewCharacter);
        EventManager.StartListening(GameEvent.EndText, NextCharacter);
        EventManager.StartListening(GameEvent.Sentencing, ExitCharacter);
    }

    public void ExitCharacter(Dictionary<string, object> data)
    {
        Debug.Log("goToHell :" + (bool)data["goToHell"]);
        character[currentChar].Exits((bool)data["goToHell"]);
        ++currentChar;

        if (currentChar >= character.Length)
        {
            currentChar = 0;
            EventManager.TriggerEvent(GameEvent.LastCharacter, new Dictionary<string, object> { });
        }
        if (clickableID != null) 
        { 
            clickableID.SetActive(false); 
        }
    }

    public void NextCharacter(Dictionary<string, object> data)
    {
        Debug.Log("switch character");
        if (!characterWaiting)
        {
            return;
        }
        //Debug.Log("switch character");
        character[currentChar].Enters(spawnPoint, spawner);

        isCharacterPresent = !isCharacterPresent;

        if (clickableID != null)
        {
            clickableID.SetActive(true);
            id.id = character[currentChar].id;
        }
        characterWaiting = false;
    }

    public void NewCharacter(Dictionary<string, object> data)
    {
        characterWaiting = true;
    }
}
