using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private BinarySpacePartitionSO binarySpacePartitionParameters;

    [SerializeField]
    [Range(0, 10)]
    private int roomOffset = 1;

    [SerializeField]
    private bool useRandomWalk = false;

    [SerializeField] private InputActionReference generate;

    public UnityEvent OnFinishedRoomGeneration;

    private Dungeon dungeon;

    protected override void RunProceduralGeneration()
    {
        Generate();
    }

    private void Generate()
    {
        dungeon.Reset();

        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(binarySpacePartitionParameters.dungeonWidth, binarySpacePartitionParameters.dungeonHeight, 0)),
            binarySpacePartitionParameters.minimumRoomWidth, binarySpacePartitionParameters.minimumRoomHeight, (int)binarySpacePartitionParameters.favoredSplitDirection);

        List<Room> rooms = new List<Room>();

        if(useRandomWalk)
        {
            rooms = CreateSWRooms(roomsList);
        }
        else
        {
            rooms = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach(var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        dungeon.Path.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private List<Room> CreateSWRooms(List<BoundsInt> roomsList)
    {
        List<Room> swRooms = new List<Room>();
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach(var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + roomOffset) 
                    && position.x <= (roomBounds.xMax - roomOffset) 
                    && position.y >= (roomBounds.yMin + roomOffset)
                    && position.y <= (roomBounds.yMax - roomOffset))
                {
                        floor.Add(position);
                }
            }
            var currentRoom = new Room(roomCenter, floor);
            swRooms.Add(currentRoom);
        }

        return swRooms;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while(roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while(position.y != destination.y)
        {
            if(destination.y > position.y)
            {
                position += Vector2Int.up;
            } else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while(position.x != destination.x)
        {
            if(destination.x > position.x)
            {
                position += Vector2Int.right;
            } else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private List<Room> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        List<Room> simpleRooms = new List<Room>();
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for(int col = roomOffset; col < room.size.x - roomOffset; col++)
            { 
                for (int row = roomOffset; row < room.size.y - roomOffset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
