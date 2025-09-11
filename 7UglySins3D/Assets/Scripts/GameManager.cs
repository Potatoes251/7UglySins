using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    Entry,
    Start,
    Paused,
    GameStart,
    Running,
}

public struct GameData
{
    public int obedience;
    public int morality;
    public int nbHell;
}

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; } 

    [Header("State Tracking")]
    [SerializeField] private GameState game_state = GameState.Entry;

    private GameData gameData;

    [SerializeField]
    private int obedienceEndingThreshold = 5;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep alive between scenes
    }

    private void Start()
    {
        Init();
       
    }

    private void Update()
    {
        MainLoop();
    }

    private void MainLoop()
    {
        switch (game_state)
        {
            case GameState.Entry:
                break;

            case GameState.Start:
                break;

            case GameState.Paused:
                break;

            case GameState.GameStart:
                break;

            case GameState.Running:
                break;

            default:
                Debug.LogWarning("Unhandled GameState: " + game_state);
                break;
        }
    }

    private void Init()
    {
        Debug.Log("GameManager Initialized");
    }

    public void UpdateState(GameState targetState)
    {
 
        Debug.Log("Game State Updated: " + game_state);

        switch (game_state)
        {
            case GameState.Entry:
                SwitchFromEntry(targetState);
                break;

            case GameState.Start:
                break;

            case GameState.Paused:
                break;

            case GameState.GameStart:
                break;

            case GameState.Running:
                break;

            default:
                Debug.LogWarning("Unhandled GameState: " + game_state);
                break;
        }
        game_state = targetState;
    }

    private void SwitchFromEntry(GameState targetState)
    {
        switch (targetState)
        {
            case GameState.Entry:
                Debug.LogWarning("switching state from entry to entry");
                break;
            case GameState.Start:
                Debug.Log("starting from entry");
                EventManager.TriggerEvent(GameEvent.NewCharacter, new Dictionary<string, object> { });
                break;

            case GameState.Paused:
                break;

            case GameState.GameStart:
                break;

            case GameState.Running:
                break;
        }
    }

    private void OnEnable()
    {
        Debug.Log("GameManager Enabled");
        EventManager.StartListening(GameEvent.ClickedStart, GameStart);
        EventManager.StartListening(GameEvent.Sentencing, Sentencing);
        EventManager.StartListening(GameEvent.LastCharacter, PickEnding);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameEvent.ClickedStart, GameStart);
    }

    private void GameStart(Dictionary<string, object> data)
    {
        UpdateState(GameState.Start);

        Debug.Log("GameManager Function Triggered");
        if (data.ContainsKey("message"))
            Debug.Log((string)data["message"]);
    }

    private void Sentencing(Dictionary<string, object> data)
    {
        Debug.Log("Sentencing : obedience : " + (int)data["obedience"] + " morality" + (int)data["morality"]);
        gameData.obedience += (int)data["obedience"];
        gameData.morality += (int)data["morality"];
        if ((bool)data["goToHell"])
        {
            gameData.nbHell++;
        }
    }

    private void PickEnding(Dictionary<string, object> data) 
    {
        Debug.Log("obedience" + gameData.obedience);
        Debug.Log("morality" + gameData.morality);
        if (gameData.obedience > obedienceEndingThreshold)
        {
            EventManager.TriggerEvent(GameEvent.Ending, new Dictionary<string, object> { { "Ending", "obedience" } });
            return;
        }
        else if (gameData.obedience < -obedienceEndingThreshold)
        {
            EventManager.TriggerEvent(GameEvent.Ending, new Dictionary<string, object> { { "Ending", "disobedience" } });
            return;
        }
        else if (gameData.morality < 0)
        {
            EventManager.TriggerEvent(GameEvent.Ending, new Dictionary<string, object> { { "Ending", "moral" } });
            return;
        }
        else
        {
            EventManager.TriggerEvent(GameEvent.Ending, new Dictionary<string, object> { { "Ending", "immoral" } });
            return;
        }

    }
}