using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkGenerator : IDungeonGenerator
{
    public Dungeon GenerateDungeon(DungeonParameters parameters)
    {
        Debug.Log("SRWGenerator:GenerateDungeon");
        Dungeon dungeon = new();

        dungeon.Rooms.AddRange(PerformRandomWalk(parameters));
        dungeon.Path = ConnectRooms(dungeon);

        return dungeon;
    }

    private List<Room> PerformRandomWalk(DungeonParameters parameters)
    {
        /*
         * #1 randomly place center points equal to the minimum room count inside dungeon RectInt boundary
         *      - points must be 2 * run length + margin away from all other points the edges of the RectInt
         * 
         * #2 run SRW at each center point 
         *
         */

        Debug.Log("SRWGenerator:PerformRandomWalk");

        List<Vector2Int> roomSeeds = GenerateRoomSeeds(parameters);
        List<Room> rooms = new();
        
        for(int i = 0; i < roomSeeds.Count; i++)
        {
            HashSet<Vector2Int> walkPath = new()
            {
                roomSeeds[i]
            };

            var previousPosition = roomSeeds[i];
            int minX = previousPosition.x;
            int minY = previousPosition.y;
            int maxX = previousPosition.x;
            int maxY = previousPosition.y;

            for (int currentIteration = 0; currentIteration < parameters.iterations; currentIteration++)
            {
                for (int step = 0; step < parameters.walkLength; step++)
                {
                    var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
                    walkPath.Add(newPosition);
                    previousPosition = newPosition;

                    if (previousPosition.x > maxX) { maxX = previousPosition.x; }
                    if (previousPosition.x < minX) { minX = previousPosition.x; }
                    if (previousPosition.y > maxY) { maxY = previousPosition.y; }
                    if (previousPosition.y < minY) { minY = previousPosition.y; }
                }

                if (parameters.walkType == RandomWalkStartType.Random)
                {
                    previousPosition = walkPath.ElementAt(Random.Range(0, walkPath.Count));
                }

                if (parameters.walkType == RandomWalkStartType.StartPoint)
                {
                    previousPosition = roomSeeds[i];
                }
            }

            var roomCenter = new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);

            // Set room center as calculated midpoint if it is contained within the walk path
            // Otherwise set it to the seed position
            Room newRoom = new(walkPath.Contains(roomCenter) ? roomCenter : roomSeeds[i], walkPath);

            if (ValidateRoom(newRoom, rooms, parameters.roomMargin))
            {
                rooms.Add(newRoom);
                Debug.Log("number of rooms: " + rooms.Count);
            } 
        }

        return rooms;
    }

    private List<Vector2Int> GenerateRoomSeeds(DungeonParameters parameters)
    {
        // Generate the list of all possible seed positions based on packing circles hexagonally into a rectangle
        // random walk with infinite iterations begins resembling circle-y thing, so this strategy should work
        // randomly select dungeon room minimum number of seeds and return that list

        var maxRoomLength = 2 * parameters.walkLength + 1;
        bool marginIsOdd = parameters.roomMargin % 2 != 0;

        List<Vector2Int> allPossibleSeeds = new();
        List<Vector2Int> chosenSeeds = new();
             
        var rows = (parameters.dungeonHeight % (maxRoomLength + parameters.roomMargin) < maxRoomLength) 
            ? parameters.dungeonHeight / (maxRoomLength + parameters.roomMargin)
            : (parameters.dungeonHeight / (maxRoomLength + parameters.roomMargin)) + 1;

        var cols = (parameters.dungeonWidth % (maxRoomLength + parameters.roomMargin) < maxRoomLength)
            ? parameters.dungeonWidth / (maxRoomLength + parameters.roomMargin)
            : (parameters.dungeonWidth / (maxRoomLength + parameters.roomMargin)) + 1;

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                Vector2Int seedPosition = parameters.startPosition + new Vector2Int(
                    ((i + 1) * parameters.walkLength) + parameters.roomMargin,
                    ((j + 1) * parameters.walkLength) + parameters.roomMargin);

                Debug.Log("Seed Position - " + seedPosition.ToString());
                allPossibleSeeds.Add(seedPosition);
            }
        }

        if(marginIsOdd)
        {
            // TODO: Add the seed positions which can fit in the spaces between the existing seeds
        }

        int roomsNeeded = parameters.minimumRooms < allPossibleSeeds.Count ? parameters.minimumRooms : allPossibleSeeds.Count;

        for(int count = 0; count < roomsNeeded; count++)
        {
            var index = Random.Range(0, allPossibleSeeds.Count);
            var selectedSeed = allPossibleSeeds[index];
            allPossibleSeeds.RemoveAt(index);
            chosenSeeds.Add(selectedSeed);
        }

        return chosenSeeds;
    }

    // This function connects all rooms by their center points
    private HashSet<Vector2Int> ConnectRooms(Dungeon dungeon)
    {
        Debug.Log("SRWGenerator:ConnectRooms");
        HashSet<Vector2Int> corridors = new();
        HashSet<int> connectedRooms = new();

        if(dungeon.Rooms.Count < 2) { 
            corridors.Add(dungeon.Rooms.First().RoomCenter); 
            return corridors;
        }

        Debug.Log("Dungeon has more than 1 room");

        var roomIndex = Random.Range(0, dungeon.Rooms.Count);
        connectedRooms.Add(roomIndex);

        while(connectedRooms.Count < dungeon.Rooms.Count)
        {
            int closestRoomIndex = FindClosestRoom(dungeon, roomIndex, connectedRooms);
            if (closestRoomIndex != -1)
            {
                connectedRooms.Add(closestRoomIndex);
                corridors.UnionWith(CreateCorridor(roomIndex, closestRoomIndex, dungeon));
                roomIndex = closestRoomIndex;
            }
        }

        return corridors;
    }

    private int FindClosestRoom(Dungeon dungeon, int roomIndex, HashSet<int> connectedRooms)
    {
        Debug.Log("SRWGenerator:FindClosestRoom");
        int closest = -1;
        float distance = float.MaxValue;

        for(int i = 0; i < dungeon.Rooms.Count; i++)
        {
            if (i == roomIndex) continue;
            if (connectedRooms.Contains(i)) continue;

            float currentDistance = Vector2Int.Distance(dungeon.Rooms[roomIndex].RoomCenter, dungeon.Rooms[i].RoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = i;
            }
        }
        return closest;
    }

    private IEnumerable<Vector2Int> CreateCorridor(int currentRoomIndex, int destinationRoomIndex, Dungeon dungeon)
    {
        Debug.Log("SRWGenerator:CreateCorridor");
        HashSet<Vector2Int> corridor = new();
        var position = dungeon.Rooms[currentRoomIndex].RoomCenter;
        var destination = dungeon.Rooms[destinationRoomIndex].RoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }

        return corridor;
    }

    private bool ValidateRoom(Room newRoom, List<Room> Rooms, int padding)
    {
        Debug.Log("SRWGenerator:ValidateRoom");
        foreach(Room room in Rooms)
        {
            if(newRoom.OverlapsWith(room, padding))
            {
                Debug.Log("SRWGenerator:ValidateRoom - False");
                return false;
            }
        }

        Debug.Log("SRWGenerator:ValidateRoom - True");
        return true;
    }
}