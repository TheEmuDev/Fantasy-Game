using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BinarySpacePartitionParameters_", menuName = "PCG/BinarySpacePartitionData")]
public class BinarySpacePartitionSO : ScriptableObject
{
    public int minimumRoomHeight = 1,
               minimumRoomWidth = 1,
               dungeonHeight = 20,
               dungeonWidth = 20;

    public Direction2D.SplitDirections favoredSplitDirection;

}
