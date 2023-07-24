using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;

    [SerializeField]
    private TileBase floorTile, wallHorizontalSingle, wallHorizontalLeft, wallHorizontalMiddle, wallHorizontalRight, wallVerticalTop, wallVerticalMiddle, wallVerticalBottom,
        wallCornerTopLeft, wallCornerTopRight, wallCornerBottomRight, wallCornerBottomLeft, wallTeeUp, wallTeeRight,wallTeeDown, wallTeeLeft, wallCross;

    public void PaintFloorTiles(Dungeon dungeon)
    {
        Debug.Log("TilemapVisualizer:PaintFloorTiles");
        HashSet<Vector2Int> dungeonFloor = dungeon.GetDungeonFloorPlan();
        PaintTiles(dungeonFloor, floorTilemap, floorTile);
    }
    public void Clear()
    {
        Debug.Log("TilemapVisualizer:Clear");
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        Debug.Log("TilemapVisualizer:PaintTiles");
        foreach(var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Debug.Log("TilemapVisualizer:PaintSingleTile");
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        Debug.Log("TilemapVisualizer:PaintSingleBasicWall");
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if(WallTypesHelper.wallHorizontalLeft.Contains(typeAsInt))
        {
            tile = wallHorizontalLeft;
        } else if (WallTypesHelper.wallHorizontalMiddle.Contains(typeAsInt))
        {
            tile = wallHorizontalMiddle;
        } else if (WallTypesHelper.wallHorizontalRight.Contains(typeAsInt))
        {
            tile = wallHorizontalRight;
        } else if (WallTypesHelper.wallVerticalTop.Contains(typeAsInt))
        {
            tile = wallVerticalTop;
        } else if (WallTypesHelper.wallVerticalMiddle.Contains(typeAsInt))
        {
            tile = wallVerticalMiddle;
        }else if (WallTypesHelper.wallVerticalBottom.Contains(typeAsInt))
        {
            tile = wallVerticalBottom;
        }else if (WallTypesHelper.wallInnerCornerTopLeft.Contains(typeAsInt))
        {
            tile = wallCornerTopLeft;
        }else if (WallTypesHelper.wallInnerCornerTopRight.Contains(typeAsInt))
        {
            tile = wallCornerTopRight;
        }else if (WallTypesHelper.wallInnerCornerBottomRight.Contains(typeAsInt))
        {
            tile = wallCornerBottomRight;
        }else if (WallTypesHelper.wallInnerCornerBottomLeft.Contains(typeAsInt))
        {
            tile = wallCornerBottomLeft;
        }else if (WallTypesHelper.wallHorizontalSingle.Contains(typeAsInt))
        {
            tile = wallHorizontalSingle;
        }    

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
        }

    internal void PaintSingleCornerWall(Vector2Int position, string neighorBinaryType)
    {
        Debug.Log("TilemapVisualizer:PaintSingleCornerWall");
        int typeAsInt = Convert.ToInt32(neighorBinaryType, 2);
        TileBase tile = null;

        if(WallTypesHelper.wallOuterCornerBottomLeft.Contains(typeAsInt))
        {
            tile = wallCornerBottomLeft;
        } else if (WallTypesHelper.wallOuterCornerBottomRight.Contains(typeAsInt))
        {
            tile = wallCornerBottomRight;
        } else if (WallTypesHelper.wallOuterCornerTopLeft.Contains(typeAsInt))
        {
            tile = wallCornerTopLeft;
        } else if (WallTypesHelper.wallOuterCornerTopRight.Contains(typeAsInt))
        {
            tile = wallCornerTopRight;
        } else if (WallTypesHelper.wallTeeUp.Contains(typeAsInt))
        {
            tile = wallTeeUp;
        } else if (WallTypesHelper.wallTeeRight.Contains(typeAsInt))
        {
            tile = wallTeeRight;
        } else if (WallTypesHelper.wallTeeDown.Contains(typeAsInt))
        {
            tile = wallTeeDown;
        } else if (WallTypesHelper.wallTeeLeft.Contains(typeAsInt))
        {
            tile = wallTeeLeft;
        }else if (WallTypesHelper.wallCross.Contains(typeAsInt))
        {
            tile = wallCross;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }
}
