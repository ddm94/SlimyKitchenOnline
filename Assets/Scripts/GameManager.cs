using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private NetworkVariable<State> state = new (State.WaitingToStart);

    private NetworkVariable<float> countdownToStartTimer = new (3f);
    private NetworkVariable<float> gamePlayingTimer = new (0f);
    [Tooltip("The duration of the game in seconds.")]
    [SerializeField] private float gamePlayingTimerMax = 60f;

    private bool isGamePaused = false;
    private bool isLocalPlayerReady;

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameManager instance.");
        }

        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;

            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //Debug.Log("SenderClientId " + serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;

        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // This player is not ready
            if (!playerReadyDictionary.ContainsKey(clientID) || !playerReadyDictionary[clientID])
            {
                allClientsReady = false;

                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }

        //Debug.Log("All clients are ready: " + allClientsReady);
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer)
            return;

        switch (state.Value)
        {
            case State.WaitingToStart:

                break;

            case State.CountdownToStart:

                countdownToStartTimer.Value -= Time.deltaTime;

                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;

                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }

                break;

            case State.GamePlaying:

                gamePlayingTimer.Value -= Time.deltaTime;

                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;
                }

                break;

            case State.GameOver:

                break;
        }

        //Debug.Log(state);
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public float GetGamePlayingTimerNormalized()
    {
        // Since for the gamePlayingTimer we are counting down (-= Time.deltaTime) instead of up
        // We reverse it with 1 -
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;

            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;


            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }
}
