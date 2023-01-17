using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNetworkManager : NetworkManager
{
    public void ReplaceCharacter(NetworkConnectionToClient conn, GameObject newPlayerObject)
    {
        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        // Include true for keepAuthority paramater to prevent ownership change
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPlayerObject), true);

        // Remove the previous player object that's now been replaced
        // Delay is required to allow replacement to complete.
        Destroy(oldPlayer, 0.1f);
    }
}