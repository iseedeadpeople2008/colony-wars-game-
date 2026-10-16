using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DragLineView : MonoBehaviour
{
    private LineRenderer line;
    private bool isActive;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.enabled = false;
        line.sortingOrder = 10;
    }

    public void Show(Vector2 from, Color color)
    {
        line.startColor = color;
        line.endColor = color;
        line.SetPosition(0, from);
        line.SetPosition(1, from);
        line.enabled = true;
        isActive = true;
    }

    public void MoveEnd(Vector2 to)
    {
        if (!isActive)
            return;

        line.SetPosition(1, to);
    }

    public void Hide()
    {
        isActive = false;
        line.enabled = false;
    }
}