using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Game Of Life/Pattern")]
public class Pattern : ScriptableObject
{
    public Vector2Int[] cells;

    public Vector2Int GetCenter()
    {
        if (cells == null || cells.Length == 0) return Vector2Int.zero;
        
        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;
        
        foreach (Vector2Int cell in cells)
        {
            min.x = Mathf.Min(cell.x, min.x);
            min.y = Mathf.Min(cell.y, min.y);
            
            max.x = Mathf.Min(cell.x, max.x);
            max.y = Mathf.Min(cell.y, max.y);
        }

        return (min + max) / 2;
    }
}
