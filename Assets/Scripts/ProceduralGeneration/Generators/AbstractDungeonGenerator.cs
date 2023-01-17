using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;

    [SerializeField]
    protected Dungeon dungeon;

    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    [SerializeField]
    protected InputActionReference generate;

    public UnityEvent OnFinishedRoomGeneration;

    private void Awake()
    {
        dungeon = FindObjectOfType<Dungeon>();
        if (dungeon == null)
            dungeon = gameObject.AddComponent<Dungeon>();

        generate.action.performed += Generate;
    }

    public void Generate(InputAction.CallbackContext obj)
    {
        Debug.Log("Generate");
        ClearDungeon();
        RunProceduralGeneration();
        OnFinishedRoomGeneration?.Invoke();
    }

    public void ClearDungeon()
    {
        dungeon.Reset();
        tilemapVisualizer.Clear();
    }

    protected abstract void RunProceduralGeneration();
}
