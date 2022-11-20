using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomDataExtractor : MonoBehaviour
{
    private Dungeon dungeon;

    public UnityEvent OnFinishedRoomProcessing;

    private void Awake()
    {
        dungeon = FindObjectOfType<Dungeon>();
    }

    public void ProcessRooms()
    {
        if (dungeon == null) return;

        foreach(Room room in dungeon.Rooms)
        {
            // find corner, near wall, and inner tiles
            foreach(Vector2Int tilePosition in room.FloorTiles)
            {
                int adjacentFloorTiles = 4;

                if(room.FloorTiles.Contains(tilePosition+Vector2Int.up) == false)
                {
                    room.WallAdjacentTilesUp.Add(tilePosition);
                    adjacentFloorTiles--;
                }

                if(room.FloorTiles.Contains(tilePosition+Vector2Int.down) == false)
                {
                    room.WallAdjacentTilesDown.Add(tilePosition);
                    adjacentFloorTiles--;
                }

                if(room.FloorTiles.Contains(tilePosition+Vector2Int.left) == false)
                {
                    room.WallAdjacentTilesLeft.Add(tilePosition);
                    adjacentFloorTiles--;
                }

                if(room.FloorTiles.Contains(tilePosition +Vector2Int.right) == false)
                {
                    room.WallAdjacentTilesRight.Add(tilePosition);
                    adjacentFloorTiles--;
                }

                // find corners
                if (adjacentFloorTiles <= 2)
                    room.CornerTiles.Add(tilePosition);

                if (adjacentFloorTiles == 4)
                    room.InnerTiles.Add(tilePosition);
            }

            room.WallAdjacentTilesUp.ExceptWith(room.CornerTiles);
            room.WallAdjacentTilesDown.ExceptWith(room.CornerTiles);
            room.WallAdjacentTilesLeft.ExceptWith(room.CornerTiles);
            room.WallAdjacentTilesRight.ExceptWith(room.CornerTiles);
        }

        Invoke(nameof(RunEvent), 1);
    }

    public void RunEvent()
    {
        OnFinishedRoomProcessing?.Invoke();
    }
}
