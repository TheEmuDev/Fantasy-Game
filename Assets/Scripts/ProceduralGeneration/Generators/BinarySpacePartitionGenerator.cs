using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitionGenerator : IDungeonGenerator
{
    public Dungeon GenerateDungeon(DungeonParameters parameters)
    {
        if (parameters.dungeonWidth <= 0 || parameters.dungeonHeight <= 0)
        {
            throw new ArgumentException("Dungeon width and height must be positive");
        }

        Dungeon dungeon = new();

        dungeon.Rooms.AddRange(BinarySpacePartition(parameters));
        dungeon.Path = ConnectRooms(dungeon);

        return dungeon;
    }

    private List<Room> BinarySpacePartition(DungeonParameters parameters)
    {
        Queue<RectInt> roomsQueue = new();
        List<RectInt> roomsList = new();

        int splitDirectionCoin = -1;

        RectInt startSpace = new(parameters.startPosition, new Vector2Int(parameters.dungeonWidth, parameters.dungeonHeight));
        roomsQueue.Enqueue(startSpace);

        while (roomsQueue.Count > 0)
        {
            var roomCandidate = roomsQueue.Dequeue();

            if (parameters.favoredSplitDirection == Direction2D.SplitDirections.Random)
            {
                splitDirectionCoin = Mathf.RoundToInt(Random.value);
            }

            if (IsRoomCandidateValid(roomCandidate, parameters))
            {
                HandleSplitDirection(roomCandidate, parameters, roomsQueue, roomsList, splitDirectionCoin);
            }
        }

        return ToRooms(roomsList, parameters);
    }

    private bool IsRoomCandidateValid(RectInt roomCandidate, DungeonParameters parameters)
    {
        return roomCandidate.size.y >= parameters.minimumRoomHeight && roomCandidate.size.x >= parameters.minimumRoomWidth;
    }

    private void HandleSplitDirection(RectInt roomCandidate, DungeonParameters parameters, Queue<RectInt> roomsQueue, List<RectInt> roomsList, int splitDirectionCoin)
    {
        bool isHorizontalSplitFavored = splitDirectionCoin == 0 || parameters.favoredSplitDirection == Direction2D.SplitDirections.Horizontal;
        bool canSplitHorizontally = roomCandidate.size.y >= parameters.minimumRoomHeight * 2;
        bool canSplitVertically = roomCandidate.size.x >= parameters.minimumRoomWidth * 2;

        if (isHorizontalSplitFavored)
        {
            if (canSplitHorizontally)
            {
                SplitRoomHorizontally(parameters.minimumRoomHeight, roomsQueue, roomCandidate);
            }
            else if (canSplitVertically)
            {
                SplitRoomVertically(parameters.minimumRoomWidth, roomsQueue, roomCandidate);
            }
            else
            {
                roomsList.Add(roomCandidate);
            }
        }
        else // Favor Vertical
        {
            if (canSplitVertically)
            {
                SplitRoomVertically(parameters.minimumRoomWidth, roomsQueue, roomCandidate);
            }
            else if (canSplitHorizontally)
            {
                SplitRoomHorizontally(parameters.minimumRoomHeight, roomsQueue, roomCandidate);
            }
            else
            {
                roomsList.Add(roomCandidate);
            }
        }
    }

    private List<Room> ToRooms(List<RectInt> roomsList, DungeonParameters parameters)
    {
        List<Room> rooms = new();
        HashSet<Vector2Int> floor = new();
        foreach(RectInt roomSpace in roomsList)
        {
            for(int col = parameters.minimumRoomPadding; col < roomSpace.size.x - parameters.minimumRoomPadding; col++)
            {
                for (int row = parameters.minimumRoomPadding; row < roomSpace.size.y - parameters.minimumRoomPadding; row++)
                {
                    Vector2Int position = roomSpace.min + new Vector2Int(row, col);
                    floor.Add(position);
                }
            }
            rooms.Add(new Room(new Vector2Int((int)roomSpace.center.x, (int)roomSpace.center.y), floor));
            floor.Clear();
        }

        return rooms;
    }

    private void SplitRoomVertically(int minimumRoomWidth, Queue<RectInt> roomsQueue, RectInt roomCandidate)
    {
        var xSplit = Random.Range(minimumRoomWidth, roomCandidate.size.x - minimumRoomWidth);
        RectInt room1 = new(roomCandidate.min, new Vector2Int(xSplit, roomCandidate.size.y));
        RectInt room2 = new(new Vector2Int(roomCandidate.min.x + xSplit, roomCandidate.min.y), new Vector2Int(roomCandidate.size.x - xSplit, roomCandidate.size.y));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private void SplitRoomHorizontally(int minimumRoomHeight, Queue<RectInt> roomsQueue, RectInt roomCandidate)
    {
        var ySplit = Random.Range(minimumRoomHeight, roomCandidate.size.y - minimumRoomHeight);
        RectInt room1 = new(roomCandidate.min, new Vector2Int(roomCandidate.size.x, ySplit));
        RectInt room2 = new(new Vector2Int(roomCandidate.min.x, ySplit), new Vector2Int(roomCandidate.size.x, roomCandidate.size.y - ySplit));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private HashSet<Vector2Int> ConnectRooms(Dungeon dungeon)
    {
        HashSet<Vector2Int> corridors = new();
        HashSet<int> connectedRooms = new();

        var roomIndex = Random.Range(0, dungeon.Rooms.Count);
        connectedRooms.Add(roomIndex);

        while(connectedRooms.Count < dungeon.Rooms.Count)
        {
            int closestRoomIndex = FindClosestRoom(dungeon, roomIndex, connectedRooms);
            connectedRooms.Add(closestRoomIndex);
            corridors.UnionWith(CreateCorridor(roomIndex, closestRoomIndex, dungeon));
            roomIndex = closestRoomIndex;
        }

        return corridors;
    }

    private int FindClosestRoom(Dungeon dungeon, int roomIndex, HashSet<int> connectedRooms)
    {
        int closest = -1;
        float distance = float.MaxValue;
        for(int i = 0; i < dungeon.Rooms.Count; i++)
        {
            if (i == roomIndex) continue;
            if (connectedRooms.Contains(i)) continue;

            float currentDistance = Vector2Int.Distance(dungeon.Rooms[roomIndex].RoomCenter, dungeon.Rooms[i].RoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = i;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateCorridor(int currentRoomIndex, int destinationRoomIndex, Dungeon dungeon)
    {
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
}
