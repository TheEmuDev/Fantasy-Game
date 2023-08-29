using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FantasyRogueLite.Lobby
{
    public class NetworkManagerLobby : NetworkManager
    {
        [SerializeField] private int minimumPlayers = 1;
        [SerializeField] private string menuScene = string.Empty;
        [SerializeField] private string gameScene = string.Empty;

        [Header("Lobby Player Prefab")]
        [SerializeField] private NetworkLobbyPlayer roomPlayerPrefab = null;

        [Header("Game Player Prefab")]
        [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;

        public List<NetworkLobbyPlayer> RoomPlayers { get; } = new List<NetworkLobbyPlayer>();
        public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();


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
            // Check if the connection has an associated player before removing from the list
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkLobbyPlayer>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }
            base.OnServerDisconnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if(SceneManager.GetActiveScene().name == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;

                NetworkLobbyPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);
                
                roomPlayerInstance.IsLeader = isLeader;
                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }

        public override void OnStopServer()
        {
            RoomPlayers.Clear();
            base.OnStopServer();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        public void StartGame()
        {
            if(SceneManager.GetActiveScene().name == menuScene)
            {
                if(!IsReadyToStart()) { return; }

                ServerChangeScene(gameScene);
            }
        }

        public override void ServerChangeScene(string newScene)
        {
            // From Menu to Game
            if (SceneManager.GetActiveScene().name == menuScene) {
                if(newScene == gameScene)
                {
                    for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                    {
                        var conn = RoomPlayers[i].connectionToClient;
                        var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                        gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                        NetworkServer.Destroy(conn.identity.gameObject);
                        NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
                    }
                }
            }

            base.ServerChangeScene(newScene);
        }

        private bool IsReadyToStart()
        {
            if(numPlayers < minimumPlayers) { return false; }

            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady) { return false;}
            }

            return true;
        }
    }
}
