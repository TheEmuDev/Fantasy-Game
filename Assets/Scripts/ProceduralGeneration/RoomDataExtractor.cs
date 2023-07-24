using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomDataExtractor : MonoBehaviour
{
    /*
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



        foreach(Room room in dungeon.GetRooms())
        {
            // find corner, near wall, and inner tiles
            foreach (Vector2Int tilePosition in room.FloorTiles)
            {
                // handle tiles which are on the dungeon path
                if (dungeon.GetPath().Contains(tilePosition)) 
                {
                    foreach (Vector2Int direction in Direction2D.cardinalDirectionsList)
                    {
                        if (room.FloorTiles.Contains(tilePosition + direction) && !dungeon.GetPath().Contains(tilePosition + direction))
                        {
                            room.PathAdjacentTiles.Add(tilePosition + direction);
                        }
                    }

                    continue; 
                }

                string neighbors = "";

                neighbors += room.FloorTiles.Contains(tilePosition + Vector2Int.up) ? "0" : "1";
                neighbors += room.FloorTiles.Contains(tilePosition + Vector2Int.right) ? "0" : "1";
                neighbors += room.FloorTiles.Contains(tilePosition + Vector2Int.down) ? "0" : "1";
                neighbors += room.FloorTiles.Contains(tilePosition + Vector2Int.left) ? "0" : "1";

                switch (neighbors)
                {
                    case "0000":
                        room.InnerTiles.Add(tilePosition);
                        break;

                    case "1000":
                        room.WallAdjacentTilesUp.Add(tilePosition);
                        break;

                    case "0100":
                        room.WallAdjacentTilesRight.Add(tilePosition);
                        break;

                    case "0010":
                        room.WallAdjacentTilesDown.Add(tilePosition);
                        break;

                    case "0001":
                        room.WallAdjacentTilesLeft.Add(tilePosition);
                        break;

                    case "1110":
                    case "1101":
                    case "1011":
                    case "0111":
                        room.DeadEndTiles.Add(tilePosition);
                        break;

                    case "1100":
                    case "0110":
                    case "0011":
                    case "1001":
                        room.CornerTiles.Add(tilePosition);
                        break;

                    case "1010":
                    case "0101":
                        room.CorridorTiles.Add(tilePosition);
                        break;

                    default:
                        Debug.LogWarning("Unreachable Tiles Detected at " + tilePosition);
                        break;
                }
            }
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

        foreach (Room room in dungeon.GetRooms())
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

            // Draw tiles with 3 adjacent walls
            Gizmos.color = Color.black;
            DrawCubes(room.DeadEndTiles);

            // Draw Tiles with 2 parallel adjacent walls
            Gizmos.color = Color.red;
            DrawCubes(room.CorridorTiles);

            Gizmos.color = new Color(255f, 150f, 0f, 1f); // dark orange
            DrawWireCubes(room.PathAdjacentTiles);
        }
    }

    private void DrawCubes(IEnumerable<Vector2Int> collection)
    {
        foreach (Vector2Int floorPosition in collection)
        {
            if (dungeon.GetPath().Contains(floorPosition)) continue;

            Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
        }
    }

    private void DrawWireCubes(IEnumerable<Vector2Int> collection)
    {
        foreach (Vector2Int floorPosition in collection)
        {
            Gizmos.DrawWireCube(floorPosition + (Vector2.one * 0.5f), Vector2.one);
        }
    }
    */
}