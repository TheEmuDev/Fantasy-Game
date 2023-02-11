using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentPlacer : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab, playerPrefab;

    [SerializeField] private int playerRoomIndex;

    [SerializeField] private List<int> roomEnemiesCount;
    [SerializeField] Dungeon dungeon;
    [SerializeField] DungeonNetworkUtil util;

    private void Awake()
    {
        if (dungeon == null) return;
    }

    public void PlaceAgents()
    {
        if (dungeon == null) return;

        // loop for each room
        for(int i = 0; i < dungeon.Rooms.Count; i++)
        {
            // analyze room tiles to find which are accessible from path
            Room room = dungeon.Rooms[i];
            RoomGraph roomGraph = new(room.FloorTiles);

            // Find the path inside the room
            HashSet<Vector2Int> roomFloor = new(room.FloorTiles);
            
            // Find the tiles belonging to both the room and the path
            roomFloor.IntersectWith(dungeon.Path);
            
            // find all the tiles in the room accessible from the path
            Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.FirstOrDefault(), room.PropPositions);

            // Positions that are reachable + path == positions where enemies can be placed
            room.PathAdjacentTiles = roomMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();

            if(roomEnemiesCount.Count > i)
            {
                PlaceEnemies(room, roomEnemiesCount[i]);
            }

            if (i == playerRoomIndex)
            {
                util.ReplaceCharacter(NetworkServer.localConnection, playerPrefab, dungeon.Rooms[i].RoomCenterPosition + Vector2.one * 0.5f);
                //GameObject player = Instantiate(playerPrefab);
                //player.transform.localPosition = dungeon.Rooms[i].RoomCenterPosition + Vector2.one * 0.5f;
                //dungeon.Players.Add(player);
            }
        }
    }

    private void PlaceEnemies(Room room, int v)
    {
        throw new NotImplementedException();
    }
}
