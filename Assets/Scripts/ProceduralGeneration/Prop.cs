using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu]
public class Prop : ScriptableObject
{
    [Header("Prop data:")]
    public Sprite PropSprite;
    public SpriteLibraryAsset propSprites;
    public SpriteResolver spriteResolver;

    /// <summary>
    /// Determines the collider size of the prop
    /// </summary>
    public Vector2Int PropSize;


    [Space, Header("Placement Type:")]
    public bool Inner = true;
    public bool Corner = true;
    public bool Corridor = true;
    public bool DeadEnd = true;
    public bool WallAdjacentUp = true;
    public bool WallAdjacentDown = true;
    public bool WallAdjacentLeft = true;
    public bool WallAdjacentRight = true;

    [Min(1)] public int QuantityMinimum;
    [Min(1)] public int QuantityMaximum;

    [Space, Header("Group Placements:")]
    public bool PlaceAsGroup = false;
    [Min(1)] public int GroupMinCount = 1;
    [Min(1)] public int GroupMaxCount = 1;
}
