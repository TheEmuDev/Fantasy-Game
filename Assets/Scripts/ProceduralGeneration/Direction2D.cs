using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public static List<Vector2Int> GetRandomCardinalDirectionList()
    {
        // Copy cardinalDirectionsList
        List<Vector2Int> shuffledList = new(cardinalDirectionsList);

        // Perform a Fisher-Yates shuffle
        for (int i = shuffledList.Count - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i
            int randomIndex = Random.Range(0, i + 1);

            // Swap elements at i and randomIndex
            (shuffledList[randomIndex], shuffledList[i]) = (shuffledList[i], shuffledList[randomIndex]);
        }

        return shuffledList;
    }
}
