using System;
using UnityEngine;

[Serializable]
public struct SerializableRectInt
{
    public Vector2Int position, size;

    public SerializableRectInt(RectInt rect)
    {
        position = rect.position;
        size = rect.size;
    }

    public RectInt ToRectInt()
    {
        return new RectInt(position, size);
    }
}
