using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNetworkUtil : NetworkBehaviour
{
    public void ReplaceCharacter(NetworkConnectionToClient conn, GameObject newPlayerObject, Vector3 spawnLocation)
    {
        if(conn.identity == null)
        {
            Debug.Log("Smiley Day");
            NetworkServer.Spawn(Instantiate(newPlayerObject, spawnLocation, Quaternion.identity), conn);
            return;
        }

        Debug.Log("Frowney Day");

        // Cache a reference to the current player object1
        GameObject oldPlayer = conn.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        // Include true for keepAuthority paramater to prevent ownership change
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPlayerObject, spawnLocation, Quaternion.identity), true);

        // Remove the previous player object that's now been replaced
        // Delay is required to allow replacement to complete.
        NetworkServer.UnSpawn(oldPlayer);
        Destroy(oldPlayer, 0.1f);
    }
}
