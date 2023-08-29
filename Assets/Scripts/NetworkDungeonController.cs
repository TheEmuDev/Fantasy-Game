using Mirror;
using UnityEngine;


public class NetworkDungeonController : NetworkBehaviour
{
    [SerializeField] 
    private DungeonParameters Parameters;

    [SerializeField]
    private TilemapVisualizer DungeonVisualizer;

    private IDungeonGenerator Generator;

    [SyncVar]
    private Dungeon sharedDungeon;

    [SerializeField]
    bool DebugLoggingEnabled;

    [Server]
    void GenerateDungeon()
    {
        if (DebugLoggingEnabled) 
            Debug.Log("NetworkDungeonController:GenerateDungeon");

        Dungeon dungeon = Generator.GenerateDungeon(Parameters);

        if (DebugLoggingEnabled)
        {
            Debug.Log("Generating Dungeon Complete");
            dungeon.PrintDungeon();
        }

        sharedDungeon = dungeon;
    }

    public override void OnStartServer()
    {
        if (DebugLoggingEnabled)
            Debug.Log("NetworkDungeonController:OnStartServer");
        base.OnStartServer();

        if(Parameters == null)
        {
            if (DebugLoggingEnabled)
                Debug.LogError("DungeonParameters have not been specified.");
            return;
        }

        Generator = Parameters.algorithm switch
        {
            DungeonGenerationAlgorithm.BinarySpacePartition => new BinarySpacePartitionGenerator(),
            DungeonGenerationAlgorithm.SimpleRandomWalk => new SimpleRandomWalkGenerator(),
            _ => null,
        };

        if (Generator == null)
        {
            if (DebugLoggingEnabled)
                Debug.LogError("Generator algorithm has not been specified.");
            return;
        }

        GenerateDungeon();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (DebugLoggingEnabled) 
        { 
            Debug.Log("NetworkDungeonController:OnStartClient - isOwned: " + isOwned);
            sharedDungeon.PrintDungeon();
        }

        DungeonVisualizer.PaintFloorTiles(sharedDungeon);
    }
}