using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FantasyRogueLite.Lobby { 
    public class NetworkGamePlayer : NetworkBehaviour
    {
        [SyncVar] private string displayName = "Loading...";

        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);
            Room.GamePlayers.Add(this);
            base.OnStartClient();
        }

        public override void OnStopClient()
        {
            Room.GamePlayers.Remove(this);
            base.OnStopClient();
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName= displayName;
        }
    }
}
