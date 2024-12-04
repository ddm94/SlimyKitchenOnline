using System;
using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance;

    public event EventHandler OnReadyChanged;

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void UpdateReadyState(ulong clientId)
    {
        UpdateReadyStateServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateReadyStateServerRpc(ulong clientId)
    {
        // Send the ready state for all players to the newly connected client
        foreach (var entry in playerReadyDictionary)
        {
            UpdateReadyStateClientRpc(clientId, entry.Key, entry.Value);
        }
    }

    [ClientRpc]
    private void UpdateReadyStateClientRpc(ulong clientId, ulong playerId, bool isReady)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Update the ready state for this specific player
            if (!playerReadyDictionary.ContainsKey(playerId))
            {
                playerReadyDictionary.Add(playerId, isReady);
            }
            else
            {
                playerReadyDictionary[playerId] = isReady;
            }

            // Notify listeners about the change
            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //Debug.Log("SenderClientId " + serverRpcParams.Receive.SenderClientId);

        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

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
            // Clean up the lobby when the game starts
            KitchenGameLobby.Instance.DeleteLobby();

            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, new EventArgs());
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}