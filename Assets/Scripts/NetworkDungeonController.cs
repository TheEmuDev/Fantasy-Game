using FantasyRogueLite.Lobby;
using Mirror;
using System.Collections;
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

    [Server]
    void GenerateDungeon()
    {
        Debug.Log("NetworkDungeonController:GenerateDungeon");
        Dungeon dungeon = Generator.GenerateDungeon(Parameters);

        Debug.Log("Generating Dungeon Complete");
        dungeon.PrintDungeon();

        sharedDungeon = dungeon;
       // Sync the dungeon with clients
       // WaitForClientReady(dungeon);
    }

    public override void OnStartServer()
    {
        Debug.Log("NetworkDungeonController:OnStartServer");
        base.OnStartServer();

        if(Parameters == null)
        {
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
            Debug.LogError("Generator algorithm has not been specified.");
            return;
        }

        GenerateDungeon();
    }

    public override void OnStartClient()
    {
        Debug.Log("NetworkDungeonController:OnStartClient - isOwned: " + isOwned);
        base.OnStartClient();
        sharedDungeon.PrintDungeon();
        DungeonVisualizer.PaintFloorTiles(sharedDungeon);
    }
}