using UnityEngine;
using System.Collections.Generic;

public static class GridUtility
{   
    /*Vector3から2に変換*/

    public static int grid_size = 100;

    // Vector3 -> Vector2Int
    public static Vector2Int ToGridPosition(Vector3 world_Pos)      //WorldToGrid
    {
        int x = Mathf.RoundToInt(world_Pos.x / grid_size);
        int y = Mathf.RoundToInt(world_Pos.z / grid_size);
        return new Vector2Int(x,y);
    }

    // Vector2Int → Vector3（ワールド座標）
    public static Vector3 ToWorldPosition(Vector2Int grid_Pos, float y = 0f)    //GridToWorld
    {
        float x = grid_Pos.x * grid_size;
        float z = grid_Pos.y * grid_size;
        return new Vector3(x, y, z);
    }

    //Vector3 → SnapされたVector3
    public static Vector3 SnapToGrid(Vector3 rawPosition, float y)
    {
        int x = Mathf.RoundToInt(rawPosition.x / grid_size);
        int z = Mathf.RoundToInt(rawPosition.z / grid_size);
        return new Vector3(x * grid_size, y, z * grid_size);
    }

}
