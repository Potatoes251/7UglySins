using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Broadcasts events and associated data to interested parties. </summary>
/// 
public enum GameEvent
{
    ClickedStart,
    NewCharacter,
    MissingWeight,
    Sentencing,
    GoNext,
    LastCharacter,
    Ending,
    StartText,
    EndText,
    PlaySfx,         // payload: { "clip": AudioClip, ["pos": Vector3] | ["follow": Transform], ["loop": bool], ["volume": float], ["pitch": float], ["spatial": float] }
    StopAllSfx,      // payload: { } (optional)
    PlayMusic,       // payload: { "clip": AudioClip, ["fade": float], ["volume": float], ["loop": bool] }
    StopMusic        // payload: { ["fade": float] }
}

[DefaultExecutionOrder(-10000)]
public class EventManager : MonoBehaviour
{
    private Dictionary<GameEvent, Action<Dictionary<string, object>>> eventDictionary;

    private static EventManager eventManager;



    public static EventManager instance
    {
        get
        {
            // if no instance is set, search for one in the scene
            if (!eventManager)
            {
                eventManager = FindFirstObjectByType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    // Still didn't find one, throw an error.
                    //Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    // initialize the event dictionary for the newly found instance And flag it
                    // so it is not destroyed on scene loading
                    eventManager.Init();
                    DontDestroyOnLoad(eventManager);
                }
            }
            return eventManager;
        }
    }

    /// <summary> Initializes the event dictionary. </summary>
    private void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<GameEvent, Action<Dictionary<string, object>>>();
        }
    }

    /// <summary> Registers a function to the event listener. </summary>
    /// <param name="eventID">  The event we are listening for. </param>
    /// <param name="listener"> The function that is subscribing. </param>
    public static void StartListening(GameEvent eventID, Action<Dictionary<string, object>> listener)
    {
        if (eventManager == null) return;

        Action<Dictionary<string, object>> thisEvent;

        if (instance.eventDictionary.TryGetValue(eventID, out thisEvent))
        {
            thisEvent += listener;
            instance.eventDictionary[eventID] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            instance.eventDictionary.Add(eventID, thisEvent);
        }
    }

    /// <summary> Unregisters a function from the event listener. </summary>
    /// <param name="eventID">  The event we were looking for. </param>
    /// <param name="listener"> The function that was listening. </param>
    public static void StopListening(GameEvent eventID, Action<Dictionary<string, object>> listener)
    {
        if (eventManager == null) return;
        if (instance.eventDictionary.TryGetValue(eventID, out Action<Dictionary<string, object>> thisEvent))
        {
            thisEvent -= listener;
            instance.eventDictionary[eventID] = thisEvent;
        }
    }

    /// <summary> Triggers an event, activating all the functions registered to the event. </summary>
    /// <param name="eventID">   The event to trigger. </param>
    /// <param name="eventData"> The data carried by the event. </param>
    public static void TriggerEvent(GameEvent eventID, Dictionary<string, object> eventData)
    {
        if (instance.eventDictionary.TryGetValue(eventID, out Action<Dictionary<string, object>> thisEvent))
        {
            thisEvent?.Invoke(eventData);
        }
    }



    private void Awake()
    {
        // Ensure exactly one instance and initialize once
        if (eventManager != null && eventManager != this)
        {
            Destroy(gameObject);
            return;
        }

        eventManager = this;
        Init();
        DontDestroyOnLoad(gameObject);
    }
}
