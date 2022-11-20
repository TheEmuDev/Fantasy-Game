using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction2D
{
    public enum SplitDirections { Horizontal, Vertical, Random }

    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static List<Vector2Int> intercardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1,1),    // UP-RIGHT   / NORTHEAST
        new Vector2Int(1,-1),   // DOWN-RIGHT / SOUTHEAST
        new Vector2Int(-1,-1),  // LEFT-DOWN  / SOUTHWEST
        new Vector2Int(-1,1)    // LEFT-UP    / NORTHWEST
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        Vector2Int.up,        // UP         / NORTH
        new Vector2Int(1,1),  // UP-RIGHT   / NORTHEAST
        Vector2Int.right,     // RIGHT      / EAST
        new Vector2Int(1,-1), // DOWN-RIGHT / SOUTHEAST
        Vector2Int.down,      // DOWN       / SOUTH
        new Vector2Int(-1,-1),// LEFT-DOWN  / SOUTHWEST
        Vector2Int.left,      // LEFT       / WEST
        new Vector2Int(-1,1)  // LEFT-UP    / NORTHWEST
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
