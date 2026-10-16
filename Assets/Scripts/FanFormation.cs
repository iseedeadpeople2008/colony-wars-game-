using UnityEngine;

public static class FanFormation
{
    public static Vector2 GetAimPoint(Vector2 targetCenter, Vector2 axis, int index, int count, float lineSpacing)
    {
        var lateral = (index - (count - 1) / 2f) * lineSpacing;
        return targetCenter + Vector2.Perpendicular(axis) * lateral;
    }
}