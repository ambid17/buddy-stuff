using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class PlayerNameController : NetworkBehaviour
{
    /// <summary>
    /// Text box used to display the name of this character.
    /// </summary>
    [Tooltip("Text box used to display the name of this character.")]
    [SerializeField]
    private TMP_Text _text;

    //Cached value for unsubscribing to save a little perf.
    private PlayerNameManager _playerNames;

    public override void OnStartClient()
    {
        //If owner is not set then do not get the name, as this does not belong to a client.
        if (!base.Owner.IsValid)
            return;

        //Get the PlayerNames instance to read this characters name(the player name).
        _playerNames = base.NetworkManager.GetInstance<PlayerNameManager>();

        //If cannot be found exit method; this shouldn't ever happen.
        if (!_playerNames.Names.TryGetValue(base.Owner, out string theName))
            return;

        _text.text = theName;

        //Also listen for updates for
        _playerNames.OnPlayerNameChanged += PlayerNamesOnOnPlayerNameChanged;
    }

    public override void OnStopClient()
    {
        //Unsubscribe from events to clean up.
        if (_playerNames != null)
            _playerNames.OnPlayerNameChanged -= PlayerNamesOnOnPlayerNameChanged;
    }


    /// <summary>
    /// Called when a player name changes after initially being set, or when added for the first time.
    /// </summary>
    private void PlayerNamesOnOnPlayerNameChanged(NetworkConnection conn, string theName)
    {
        //If the name being changed is not for this owner then do not update anything.
        if (conn != base.Owner)
            return;

        //Set new name.
        _text.text = theName;
    }
}
