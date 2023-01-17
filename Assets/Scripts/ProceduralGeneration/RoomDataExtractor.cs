using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomDataExtractor : MonoBehaviour
{
    private Dungeon dungeon;

    
    [SerializeField]
    private bool showGizmo = false; // FOR DEBUG 

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

                if(room.FloorTiles.Contains(tilePosition+Vector2Int.right) == false)
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

    private void OnDrawGizmosSelected()
    {
        if(dungeon == null || !showGizmo) return;

        foreach (Room room in dungeon.Rooms)
        {
            // Draw inner tiles
            Gizmos.color = Color.yellow;
            DrawCubes(room.InnerTiles);
            
            // Draw tiles adjacent to wall up
            Gizmos.color = Color.blue;
            DrawCubes(room.WallAdjacentTilesUp);

            // Draw tiles adjacent to wall down
            Gizmos.color = Color.green;
            DrawCubes(room.WallAdjacentTilesDown);

            // Draw tiles adjacent to wall left
            Gizmos.color = Color.cyan;
            DrawCubes(room.WallAdjacentTilesLeft);

            // Draw tiles adjacent to wall right
            Gizmos.color = Color.white;
            DrawCubes(room.WallAdjacentTilesRight);

            // Draw corner tiles
            Gizmos.color = Color.magenta;
            DrawCubes(room.CornerTiles);
        }
    }

    private void DrawCubes(IEnumerable<Vector2Int> collection)
    {
        foreach (Vector2Int floorPosition in collection)
        {
            if (dungeon.Path.Contains(floorPosition)) continue;

            Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
        }
    }
}
