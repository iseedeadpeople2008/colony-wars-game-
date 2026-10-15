using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Polygon
{
    private readonly Vector2[] points;

    public IReadOnlyList<Vector2> Points => points;
    public Rect Bounds { get; }

    public Polygon(IReadOnlyList<Vector2> source)
    {
        if (source == null || source.Count < 3)
            throw new ArgumentException("Polygon needs at least 3 points");

        points = new Vector2[source.Count];
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        for (int i = 0; i < source.Count; i++)
        {
            points[i] = source[i];
            minX = Mathf.Min(minX, source[i].x);
            minY = Mathf.Min(minY, source[i].y);
            maxX = Mathf.Max(maxX, source[i].x);
            maxY = Mathf.Max(maxY, source[i].y);
        }

        Bounds = Rect.MinMaxRect(minX, minY, maxX, maxY);
    }
}