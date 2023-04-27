using FantasyRogueLite.Lobby;
using Mirror;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FantasyRogueLite.Lobby
{
    public class NetworkManagerLobby : NetworkManager
    {
        [SerializeField] private string menuScene = string.Empty;

        [Header("Room")]
        [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                if(!spawnPrefabs.Contains(prefab))
                    spawnPrefabs.Add(prefab);
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if(numPlayers >= maxConnections)
            {
                Debug.Log("Number of players is equal or greater than max connections allowed");
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().name != menuScene) 
            { 
                Debug.Log("Active Sceen: " + SceneManager.GetActiveScene().name + " does not match " + menuScene);
                // If we want to have drop in - drop out co-op. We will need to change this
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if(SceneManager.GetActiveScene().name == menuScene)
            {
                NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }
    }
}
