using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.intercardinalDirectionsList);
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
    }

    private static void CreateBasicWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighborBinaryType = "";
            foreach(var direction in Direction2D.cardinalDirectionsList)
            {
                var neighborPosition = position + direction;
                if(floorPositions.Contains(neighborPosition))
                {
                    neighborBinaryType += "1";
                } else
                {
                    neighborBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighborBinaryType);
        }
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach(var position in cornerWallPositions)
        {
            string neighorBinaryType = "";
            foreach(var direction in Direction2D.eightDirectionsList)
            {
                var neighorPosition = position + direction;
                if(floorPositions.Contains(neighorPosition))
                {
                    neighorBinaryType += "1";
                } else
                {
                    neighorBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintSingleCornerWall(position, neighorBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighborPosition = position + direction;
                if(!floorPositions.Contains(neighborPosition))
                {
                    wallPositions.Add(neighborPosition);
                }
            }
        }

        return wallPositions;
    }
}
