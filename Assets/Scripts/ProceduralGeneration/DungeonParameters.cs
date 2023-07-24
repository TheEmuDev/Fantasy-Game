using UnityEngine;

public enum DungeonGenerationAlgorithm
{
    BinarySpacePartition,
    SimpleRandomWalk
}

public enum RandomWalkStartType
{
    Random,
    StartPoint,
    PreviousWalkEnd
}

[CreateAssetMenu(fileName = "DungeonParameters_", menuName = "Proc-Gen/DungeonParameters")]
public class DungeonParameters : ScriptableObject
{
    public DungeonGenerationAlgorithm algorithm;

    [Tooltip("Starting point for dungeon generation")]
    public Vector2Int startPosition;

    [Tooltip("Width of the dungeon to be generated")]
    [Min(15)]
    public int dungeonWidth;

    [Tooltip("Height of the dungeon to be generated")]
    [Min(15)]
    public int dungeonHeight;

    [Tooltip("Minimum padding between rooms")]
    [Min(1)]
    public int minimumRoomPadding = 1;

    [Tooltip("Minimum number of rooms to generate")]
    [Min(1)]
    public int minimumRooms = 1;

    [Header("Simple Random Walk Parameters")]
    [Tooltip("Number of walk iterations for the random walk algorithm")]
    [Min(1)]
    public int iterations = 10;

    [Tooltip("Length of each walk in the random walk algorithm")]
    [Min(1)]
    public int walkLength = 10;

    [Tooltip("Determines the starting point for each iteration of the Simple Random Walk algorithm")]
    public RandomWalkStartType walkType;

    [Header("Binary Space Partition Parameters")]
    [Tooltip("Minimum width of a room within the dungeon")]
    [Min(5)]
    public int minimumRoomWidth;

    [Tooltip("Minimum height of a room within the dungeon")]
    [Min(5)]
    public int minimumRoomHeight;

    [Tooltip("Preferred direction to split the space during the Binary Space Partition algorithm")]
    public Direction2D.SplitDirections favoredSplitDirection;
}