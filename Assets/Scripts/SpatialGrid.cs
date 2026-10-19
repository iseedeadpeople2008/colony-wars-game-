using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    private readonly float cellSize;
    private readonly Vector2 origin;
    private readonly int width;
    private readonly int height;
    private readonly int[] cellHead;
    private int[] next = new int[1023];

    public int Width => width;
    public int Height => height;

    public SpatialGrid(Rect bounds, float cellSize)
    {
        this.cellSize = cellSize;
        origin = bounds.min;
        width = Mathf.Max(Mathf.CeilToInt(bounds.width / cellSize), 1);
        height = Mathf.Max(Mathf.CeilToInt(bounds.height / cellSize), 1);
        cellHead = new int[width * height];
    }

    public void Rebuild(List<Soldier> soldiers)
    {
        for (int i = 0; i < cellHead.Length; i++)
        {
            cellHead[i] = -1; 
        }

        for (int i = 0; i < soldiers.Count; i++)
        {
            int cx = CellX(soldiers[i].Position);
            int cy = CellY(soldiers[i].Position);
            int index = cy * width + cx;
            next[i] = cellHead[index];
            cellHead[index] = i;
        }
    }

    public int CellX(Vector2 position)
    {
        return Mathf.Clamp((int)((position.x - origin.x) / cellSize), 0, width - 1);
    }

    public int CellY(Vector2 position)
    {
        return Mathf.Clamp((int)((position.y - origin.y) / cellSize), 0, height - 1);
    }

    public int First(int cx, int cy)
    {
        if (cx < 0 || cx >= width || cy < 0 || cy >= height)
            return -1;

        return cellHead[cy * width + cx];
    }

    public int Next(int soldierIndex)
    {
        return next[soldierIndex];
    }
}