using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


/*
 * Create Corridor
 * Save Corridor Info
 * Create Rooms
 * Save Room Info
 * 
 * 
 */

public class CorridorFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [Header("Parameters")]
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField][Range(0.1f, 1)] private float roomPercent = 0.8f;
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

        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions, potentialRoomPositions);
        
        dungeon.Path.UnionWith(floorPositions);
        
        List<Room> dungeonRooms = CreateRooms(potentialRoomPositions);
        
        foreach(Room room in dungeonRooms)
        {
            dungeon.Rooms.Add(room);
        }

        List<Vector2Int> deadEnds = FindDeadEnds(floorPositions);
        CreateRoomsAtDeadEnds(deadEnds, dungeonRooms);

        tilemapVisualizer.PaintFloorTiles(dungeon);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private List<Room> CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, List<Room> rooms)
    {
        foreach (var position in deadEnds)
        {
            bool foundInRooms = false;
            foreach (var room in rooms)
            {
                if(room.FloorTiles.Contains(position))
                {
                    foundInRooms = true;
                    break;
                }
            }

            if (!foundInRooms)
            {
                var deadEndRoom = RunRandomWalk(randomWalkParameters, position);
                rooms.Add(new Room(position, deadEndRoom));
            }
        }

        return rooms;
    }

    private List<Vector2Int> FindDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach(Vector2Int pos in floorPositions)
        {
            int neighborCount = 0;
            foreach(var direction in Direction2D.cardinalDirectionsList)
            {
                if(floorPositions.Contains(pos + direction))
                    neighborCount++;
            }
            if(neighborCount == 1)
                deadEnds.Add(pos);
        }
        return deadEnds;
    }

    private List<Room> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        List<Room> dungeonRooms = new List<Room>();
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            dungeonRooms.Add(new Room(roomPosition, roomFloor));
        }

        return dungeonRooms;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
    }
}
