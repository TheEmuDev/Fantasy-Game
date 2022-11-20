using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon()
    {
        ClearDungeon();
        RunProceduralGeneration();
    }

    public void ClearDungeon()
    {
        tilemapVisualizer.Clear();
    }

    protected abstract void RunProceduralGeneration();
}
