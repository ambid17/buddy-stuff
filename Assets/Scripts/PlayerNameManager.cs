using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerNameManager : NetworkBehaviour
{
    /* Since this is a syncType it will automatically be synchronized to clients
     * whenever it updates, and when clients spawn this object! */

    /// <summary>
    /// Called when a player name is updated.
    /// </summary>
    public event Action<NetworkConnection, string> OnPlayerNameChanged;

    /// <summary>
    /// Names of all connected clients.
    /// </summary>
    public readonly SyncDictionary<NetworkConnection, string> Names = new();

    public readonly SyncDictionary<string, bool> UsedNames = new SyncDictionary<string, bool>() {
        { "Pedro", false },
        { "Bob", false },
        { "Greg", false },
        { "Larry", false }
    };

    private void Awake()
    {
        Names.OnChange += NamesOnOnChange;
    }

    public override void OnStartNetwork()
    {
        //Register this to the NetworkManager so it can be found easily by any script!
        NetworkManager.RegisterInstance(this);
    }

    public override void OnStartServer()
    {
        ServerManager.OnRemoteConnectionState += ServerRemoteConnectionStateChanged;
    }

    public override void OnStopServer()
    {
        ServerManager.OnRemoteConnectionState -= ServerRemoteConnectionStateChanged;
    }

    public override void OnStopNetwork()
    {
        //Unregister to clean up.
        if (base.NetworkManager != null)
            base.NetworkManager.UnregisterInstance<PlayerNameManager>();
    }

    private void ServerRemoteConnectionStateChanged(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        //If disconnecting remove from the dictionary.
        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            Names.Remove(conn);
        }
        //If connecting then add.
        else if (args.ConnectionState == RemoteConnectionState.Started)
        {
            /* When a player connects assign them a random name. */

            var randomUnusedName = UsedNames.Where(pair => pair.Value == false).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            string randomName = "defaultName";
            if(randomUnusedName.Key == null)
            {
                randomName = Random.Range(0, 9999).ToString();
            }
            else
            {
                randomName = randomUnusedName.Key;
            }

            Names.Add(conn, randomName);
        }
    }

    /// <summary>
    /// Calls whenever the _names collection updates.
    /// </summary>
    private void NamesOnOnChange(SyncDictionaryOperation op, NetworkConnection key, string value, bool asserver)
    {
        //If an add or modify then invoke.
        if (op == SyncDictionaryOperation.Add || op == SyncDictionaryOperation.Set)
            OnPlayerNameChanged?.Invoke(key, value);
    }

    /// <summary>
    /// Allows a client to call this RPC, updating their name.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerSetName(string newName, NetworkConnection caller = null)
    {
        //Caller will never be null; the server will assign it automatically when a client calls this since RequireOwnership is false.
        // ReSharper disable once AssignNullToNotNullAttribute
        Names[caller] = newName;
    }
}
