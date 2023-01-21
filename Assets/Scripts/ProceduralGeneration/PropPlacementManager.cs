using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PropPlacementManager : MonoBehaviour
{
    Dungeon dungeon;

    [SerializeField] 
    private List<Prop> props;
    
    [SerializeField, Range(0, 1)]
    private float cornerPropPlacementRate = 0.7f;

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinished;

    private void Awake()
    {
        dungeon = FindObjectOfType<Dungeon>();
    }

    public void ProcessRooms()
    {
        if (dungeon == null) return;

        foreach (Room room in dungeon.Rooms)
        {
            // Place props in the corners
            List<Prop> cornerProps = props.Where(x => x.Corner).ToList();
            PlaceCornerProps(room, cornerProps);

            // Place props along the left wall
            List<Prop> leftWallProps = props.Where(x => x.WallAdjacentLeft).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, leftWallProps, room.WallAdjacentTilesLeft, PlacementOriginCorner.BottomLeft);

            // Place props along the right wall
            List<Prop> rightWallProps = props.Where(x => x.WallAdjacentRight).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, rightWallProps, room.WallAdjacentTilesRight, PlacementOriginCorner.TopRight);

            // Place props along upper wall
            List<Prop> upperWallProps = props.Where(x => x.WallAdjacentUp).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, upperWallProps, room.WallAdjacentTilesUp, PlacementOriginCorner.TopLeft);

            // Place props along the lower wall
            List<Prop> lowerWallProps = props.Where(x => x.WallAdjacentDown).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, lowerWallProps, room.WallAdjacentTilesDown, PlacementOriginCorner.BottomLeft);

            // Place props on inner room tiles
            List<Prop> innerProps = props.Where(x => x.Inner).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
        }

        Invoke(nameof(RunEvent), 1);
    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }

    /// <summary>
    /// Places props near walls
    /// </summary>
    /// <param name="room"></param>
    /// <param name="props"></param>
    /// <param name="availableTiles"></param>
    /// <param name="placement"></param>
    private void PlaceProps(Room room, List<Prop> props, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        // Remove path positions from the initial wall adjacent tiles to prevent placements which
        // block the player from progressing
        HashSet<Vector2Int> tempPositions = new(availableTiles);
        tempPositions.ExceptWith(dungeon.Path);

        // Attempt to place all the props
        foreach (Prop prop in props)
        {
            // quantity control
            int quantity = UnityEngine.Random.Range(prop.QuantityMinimum, prop.QuantityMaximum+1);

            for (int i = 0; i < quantity; i++)
            {
                // remove taken position
                tempPositions.ExceptWith(room.PropPositions);
                // shuffle the positions
                List<Vector2Int> availablePositions = tempPositions.OrderBy(x => Guid.NewGuid()).ToList();
                // If placement has failed there is no point in trying to place the same prop again
                if (PlaceProp(room, prop, availablePositions, placement) == false)
                    break;
            }
        }
    }

    /// <summary>
    /// Attempt to place the prop 
    /// </summary>
    /// <param name="room"></param>
    /// <param name="prop"></param>
    /// <param name="availablePositions"></param>
    /// <param name="placement"></param>
    /// <returns></returns>
    private bool PlaceProp(Room room, Prop prop, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        for (int i = 0; i < availablePositions.Count; i++)
        {
            // select the specified position
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            // check if there is enough space around to fit the prop
            List<Vector2Int> freePositionsAround = TryToFitProp(prop, availablePositions, position, placement);

            // if there is enough space around to fit the prop
            if (freePositionsAround.Count == prop.PropSize.x * prop.PropSize.y)
            {
                // Place the gameObject
                PlacePropAt(room, position, prop);

                // Lock all the positoins required by the prop (based on its size)
                foreach (Vector2Int pos in freePositionsAround)
                {
                    room.PropPositions.Add(pos);
                }

                // Deal with groups
                if (prop.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, prop, 1);
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room"></param>
    /// <param name="groupStartPosition"></param>
    /// <param name="prop"></param>
    /// <param name="searchOffset"></param>
    private void PlaceGroupObject(Room room, Vector2Int groupStartPosition, Prop prop, int searchOffset)
    {
        // Not intended for large props

        // Calculate how many elements are in the group -1 that we have placed in the center 
        int count = UnityEngine.Random.Range(prop.GroupMinCount, prop.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);

        // find available spaces around the center point
        // searchOffset is used to limit the distance between those points and the center point
        List<Vector2Int> availableSpaces = new();
        for(int xOffset = -searchOffset; xOffset <= searchOffset; xOffset++)
        {
            for (int yOffset = -searchOffset; yOffset <= searchOffset; yOffset++)
            {
                Vector2Int tempPos = groupStartPosition + new Vector2Int(xOffset, yOffset);
                if(room.FloorTiles.Contains(tempPos) &&
                    !dungeon.Path.Contains(tempPos) &&
                    !room.PropPositions.Contains(tempPos))
                {
                    availableSpaces.Add(tempPos);
                }
            }
        }

        // Shuffle the list
        availableSpaces.OrderBy(x => Guid.NewGuid());
        
        // place props (or as many as space allows up to maximum)
        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropAt(room, availableSpaces[i], prop);
        }
    }

    /// <summary>
    /// Place prop as new GameObject at specific location
    /// </summary>
    /// <param name="room"></param>
    /// <param name="placementPosition"></param>
    /// <param name="prop"></param>
    /// <returns></returns>
    private GameObject PlacePropAt(Room room, Vector2Int placementPosition, Prop prop)
    {
        // Instantiate the prop at this position
        GameObject propObject = Instantiate(propPrefab);
        SpriteRenderer propSpriteRenderer = propObject.GetComponentInChildren<SpriteRenderer>();

        // Set the sprite
        propSpriteRenderer.sprite = prop.PropSprite;

        // Add a collider
        CapsuleCollider2D collider = propSpriteRenderer.gameObject.GetComponentInChildren<CapsuleCollider2D>();
        collider.offset = Vector2.zero;
        if(prop.PropSize.x > prop.PropSize.y)
        {
            collider.direction = CapsuleDirection2D.Horizontal;
        }
        Vector2 size = new(prop.PropSize.x * 0.8f, prop.PropSize.y * 0.8f);
        collider.size = size;

        // adjust the position to the sprite
        propObject.transform.localPosition = (Vector2)placementPosition;
        propSpriteRenderer.transform.localPosition = (Vector2)prop.PropSize * 0.5f;

        // save the prop in the room data
        room.PropPositions.Add(placementPosition);
        room.Props.Add(propObject);
        return propObject;
    }

    /// <summary>
    /// Checks if prop will fit based on its size
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="availablePositions"></param>
    /// <param name="startPosition"></param>
    /// <param name="placement"></param>
    /// <returns></returns>
    private List<Vector2Int> TryToFitProp(Prop prop, List<Vector2Int> availablePositions, Vector2Int startPosition, PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        if(placement == PlacementOriginCorner.BottomLeft)
        {
            for(int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for(int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    CheckIfTileIsAvailable(freePositions, availablePositions, startPosition, xOffset, yOffset);
                }
            }
        } else if (placement == PlacementOriginCorner.BottomRight)
        {
            for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    CheckIfTileIsAvailable(freePositions, availablePositions, startPosition, xOffset, yOffset);
                }
            }
        } else if (placement == PlacementOriginCorner.TopLeft)
        {
            for(int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for(int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    CheckIfTileIsAvailable(freePositions, availablePositions, startPosition, xOffset, yOffset);
                }
            }    
        } else
        {
            for(int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for(int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    CheckIfTileIsAvailable(freePositions, availablePositions, startPosition, xOffset, yOffset);
                }
            }
        }

        return freePositions;
    }

    private void CheckIfTileIsAvailable(List<Vector2Int> freePositions, List<Vector2Int> availablePositions, Vector2Int target, int xOffset, int yOffset)
    {
        Vector2Int tempPos = target + new Vector2Int(xOffset, yOffset);
        if (availablePositions.Contains(tempPos))
            freePositions.Add(tempPos);
    }

    private void PlaceCornerProps(Room room, List<Prop> cornerProps)
    {
        float placementRate = cornerPropPlacementRate;
        foreach(Vector2Int cornerTile in room.CornerTiles)
        {
            if (UnityEngine.Random.value < placementRate)
            {
                Prop prop = cornerProps[UnityEngine.Random.Range(0, cornerProps.Count)];

                PlacePropAt(room, cornerTile, prop);

                if(prop.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, prop, 2);
                }
            } else
            {
                placementRate = Mathf.Clamp01(placementRate + 0.1f);
            }

        }
    }
}

/// <summary>
/// Determines which square to check 
/// first when prop is bigger than one tile
/// </summary>
public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}
