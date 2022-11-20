using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGraph 
{
    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();

    public RoomGraph(HashSet<Vector2Int> roomFloor)
    {
        foreach(Vector2Int pos in roomFloor)
        {
            List<Vector2Int> adjacentTiles = new List<Vector2Int>();
            foreach(Vector2Int direction in Direction2D.cardinalDirectionsList)
            {
                Vector2Int newPos = pos + direction;
                if (roomFloor.Contains(newPos))
                {
                    adjacentTiles.Add(newPos);
                }
            }
            graph.Add(pos, adjacentTiles);
        }
    }

    /// <summary>
    /// Creates a map of reachable tiles using breadth first search (BFS)
    /// </summary>
    /// <param name="startPos">Door position or starting tile position on path between rooms</param>
    /// <param name="occupiedNodes"></param>
    /// <returns></returns>
    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
    {
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>
        {
            startPos
        };

        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>
        {
            { startPos, startPos }
        };

        while (nodesToVisit.Count > 0)
        {
            // get data about specific position
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> adjacentTiles = graph[node];

            // loop through adjacent positions
            foreach(Vector2Int adjacentTile in adjacentTiles)
            {
                // add the adjacent position to the map if its valid
                if(visitedNodes.Contains(adjacentTile) == false &&
                    occupiedNodes.Contains(adjacentTile) == false)
                {
                    nodesToVisit.Enqueue(adjacentTile);
                    visitedNodes.Add(adjacentTile);
                    map[adjacentTile] = node;
                }
            }
        }

        return map;
    }
} 
