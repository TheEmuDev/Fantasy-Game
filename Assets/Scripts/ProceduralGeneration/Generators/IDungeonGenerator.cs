using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonGenerator
{
    Dungeon GenerateDungeon(DungeonParameters parameters);
}
