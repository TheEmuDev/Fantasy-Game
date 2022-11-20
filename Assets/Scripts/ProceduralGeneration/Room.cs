using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all data associated with a dungeon room
/// </summary>
public class Room
{
    public Vector2 RoomCenterPosition { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> WallAdjacentTilesUp { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> WallAdjacentTilesDown { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> WallAdjacentTilesLeft { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> WallAdjacentTilesRight { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> PropPositions { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> Props { get; set; } = new List<GameObject>();
    public List<Vector2Int> PathAdjacentTiles { get; set; } = new List<Vector2Int>();
    public List<GameObject> Enemies { get; set; } = new List<GameObject>();

    public Room(Vector2 roomCenterPosition, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPosition = roomCenterPosition;
        this.FloorTiles = floorTiles;
    }
}