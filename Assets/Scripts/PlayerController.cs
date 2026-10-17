using System;
using UnityEngine;

public class PlayerController : IDisposable
{
    public event Action<Territory> GestureStarted;
    public event Action GestureEnded;
    public event Action<Territory, Territory> TargetSelected;

    private readonly GameWorld world;
    private readonly DragInputHandler input;
    private readonly float hitRadius;

    private Territory source;

    public PlayerController(GameWorld world, DragInputHandler input, float hitRadius)
    {
        this.world = world;
        this.input = input;
        this.hitRadius = hitRadius;

        input.DragStarted += OnDragStarted;
        input.DragEnded += OnDragEnded;
    }

    public void Dispose()
    {
        input.DragStarted -= OnDragStarted;
        input.DragEnded -= OnDragEnded;
    }

    private void OnDragStarted(Vector2 point)
    {
        Territory selected = world.FindTerritoryAt(point, hitRadius);
        if (selected != null && selected.OwnerId == Faction.Player)
        {
            source = selected;
            GestureStarted?.Invoke(source);
        }
    }

    private void OnDragEnded(Vector2 point)
    {
        if (source == null)
            return;

        Territory from = source;
        source = null;
        GestureEnded?.Invoke();

        Territory to = world.FindTerritoryAt(point, hitRadius);
        if (to == null || to == from)
            return;

        if (from.OwnerId != Faction.Player)
            return;

        TargetSelected?.Invoke(from, to);
    }
}